using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		private readonly Type _interfaceType;
		private readonly IMessageDispatcher _messageDispatcher;
	    private readonly IMessageFactory _messageFactory;

	    private readonly IDurableConnection _durableConnection;
        private readonly Dictionary<MethodInfo, TimeSpan> _methodTimeouts = new Dictionary<MethodInfo, TimeSpan>();
        RemoteCancellationTokenSource _tokenSource;

        public TwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher messageDispatcher, IMessageFactory messageFactory, Type interfaceType)
		{
			_channel = channel;
            _durableConnection = _channel as IDurableConnection;
			_messageDispatcher = messageDispatcher;
            _interfaceType = interfaceType;
		    TimeSpan defaultTimeout =
		        (interfaceType.GetCustomAttributes(typeof(TimeoutAttribute), true).FirstOrDefault() as TimeoutAttribute)?
		        .Timeout ?? DefaultConfig.DefaultTimeout;
		    foreach (MethodInfo info in interfaceType.GetMethods())
		    {
		        TimeSpan? newTimeout =
		            (info.GetCustomAttributes(typeof(TimeoutAttribute), true).FirstOrDefault() as TimeoutAttribute)?.Timeout;
		        _methodTimeouts[info] = newTimeout ?? defaultTimeout;
		    }
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
                    handler.WaitForResponse(_methodTimeouts[invocation.Method], CancellationToken.None);
			    }
			    else
			    {
			        do
                    {
                        SendMessage(invocation, handler);
                        WaitForHandler(handler, _methodTimeouts[invocation.Method]);
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
	        _channel.Send(_messageFactory.CreateRequestMessage(handler.HandledMessageType, _interfaceType.Name,
	            invocation.Method.Name, invocation.Arguments, true));
	    }

	    private void WaitForHandler(IResponseHandler handler, TimeSpan timeout)
	    {
	        var tokenSource = _tokenSource;
	        handler.WaitForResponse(timeout, tokenSource.Token);
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