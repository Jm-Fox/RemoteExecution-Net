using System.Threading;
using AopAlliance.Intercept;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Handlers;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Remoting
{
	internal class TwoWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IOutputChannel _channel;
		private readonly string _interfaceName;
		private readonly IMessageDispatcher _messageDispatcher;
	    private readonly IMessageFactory _messageFactory;

	    private readonly IDurableConnection _durableConnection;
        RemoteCancellationTokenSource _tokenSource;

        public TwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher messageDispatcher, IMessageFactory messageFactory, string interfaceName)
		{
			_channel = channel;
            _durableConnection = _channel as IDurableConnection;
			_messageDispatcher = messageDispatcher;
			_interfaceName = interfaceName;
		    _messageFactory = messageFactory;
		    GenerateNewCancellationToken();
            if (_durableConnection != null)
		    {
		        _durableConnection.ConnectionAborted += () =>
		        {
                    _tokenSource.Aborted = true;
		            _tokenSource.Cancel();
                };
		        _durableConnection.ConnectionRestored += () =>
		        {
                    _tokenSource.Restored = true;
		            GenerateNewCancellationToken();
		        };
		    }
		}

	    private void GenerateNewCancellationToken()
	    {
	        _tokenSource = new RemoteCancellationTokenSource();
	    }

	    #region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();

			_messageDispatcher.Register(handler);
			try
			{
			    if (_durableConnection == null)
                {
                    SendMessage(invocation, handler);
                    handler.WaitForResponse(DefaultConfig.DefaultTimeout, CancellationToken.None);
			    }
			    else
			    {
			        do
                    {
                        SendMessage(invocation, handler);
                        WaitForHandler(handler);
                    } while (!handler.HasValue);
			    }
			}
			finally
			{
				_messageDispatcher.Unregister(handler.HandledMessageType);
			}
            
            (handler as IIncomplete)?.Complete(invocation.Method);
            return handler.GetValue();
		}

	    private void SendMessage(IMethodInvocation invocation, IResponseHandler handler)
	    {
	        _channel.Send(_messageFactory.CreateRequestMessage(handler.HandledMessageType, _interfaceName,
	            invocation.Method.Name, invocation.Arguments, true));
	    }

	    private void WaitForHandler(IResponseHandler handler)
	    {
	        var tokenSource = _tokenSource;
	        handler.WaitForResponse(DefaultConfig.DefaultTimeout, tokenSource.Token);
            if (tokenSource.IsCancellationRequested)
            {
                if (tokenSource.Aborted)
                    throw new ConnectionOpenException("Connection was closed.");
                // No action required for connection restored.
            }
	    }

	    #endregion

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel.Id);
		}
	}
}