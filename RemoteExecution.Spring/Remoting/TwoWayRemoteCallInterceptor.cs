using System;
using System.Diagnostics;
using System.Threading;
using AopAlliance.Intercept;
using RemoteExecution.Channels;
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
	    private readonly RemoteExecutionPolicies _policies;
        protected RemoteCancellationTokenSource _tokenSource;

        public TwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher messageDispatcher, IMessageFactory messageFactory, string interfaceName, RemoteExecutionPolicies policies)
        {
            _interfaceName = interfaceName;
			_channel = channel;
            _durableConnection = _channel as IDurableConnection;
			_messageDispatcher = messageDispatcher;
		    _policies = policies;
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
                    _tokenSource.Cancel();
                    GenerateNewCancellationToken();
		        };
		        _durableConnection.ConnectionInterrupted += () =>
		        {
		            _tokenSource.Cancel();
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
		    var policy = _policies[invocation.Method];

            _messageDispatcher.Register(handler);
			try
			{
			    if (_durableConnection == null)
                {
                    SendMessage(invocation, handler);
                    handler.WaitForResponse(policy.Timeout, CancellationToken.None);
			    }
			    else
			    {
			        if (policy.TimeoutIsStrict)
			            SendDurableStrict(invocation, handler, policy);
			        else 
			            SendDurable(invocation, handler, policy);
			    }
			}
			finally
			{
				_messageDispatcher.Unregister(handler.HandledMessageType);
			}
            
            (handler as IIncomplete)?.Complete(invocation.Method);
            return handler.GetValue();
		}

	    private bool SendMessage(IMethodInvocation invocation, IResponseHandler handler)
	    {
	        return _channel.Send(_messageFactory.CreateRequestMessage(handler.HandledMessageType, _interfaceName,
	            invocation.Method.Name, invocation.Arguments, true));
	    }

	    private void SendDurable(IMethodInvocation invocation, IResponseHandler handler, RemoteExecutionPolicy policy)
	    {
	        var sent = true;
	        var timeout = policy.Timeout;
            while (true)
            {
                if (sent)
                    sent = SendMessage(invocation, handler);
                var tokenSource = _tokenSource;
	            handler.WaitForResponse(timeout, tokenSource.Token);
	            if (tokenSource.IsCancellationRequested)
	            {
	                if (tokenSource.Aborted)
	                    throw new ConnectionOpenException("Connection was closed.");
                    sent = !sent;
                    if (!tokenSource.Restored)
                        tokenSource.Restored = false;
                    continue;
	            }
	            if (!handler.HasValue)
	                // Cancellation not requested but value not received. This means that the operation timed out.
	                throw new TimeoutException();
	            break;
	        }
        }

        private void SendDurableStrict(IMethodInvocation invocation, IResponseHandler handler, RemoteExecutionPolicy policy)
        {
            var sent = true;
            var timeout = policy.Timeout;
            var clock = new Stopwatch();
            while (true)
            {
                if (sent)
                    sent = SendMessage(invocation, handler);
                var tokenSource = _tokenSource;
                clock.Start();
                handler.WaitForResponse(timeout, tokenSource.Token);
                clock.Stop();
                if (tokenSource.IsCancellationRequested)
                {
                    if (tokenSource.Aborted)
                        throw new ConnectionOpenException("Connection was closed.");
                    // Presently no difference between Restored / Interrupted.
                    timeout = timeout - clock.Elapsed;
                    clock.Reset();
                    sent = !sent;
                    if (!tokenSource.Restored)
                        tokenSource.Restored = false;
                    continue;
                }
                if (!handler.HasValue)
                    // Cancellation not requested but value not received. This means that the operation timed out.
                    throw new TimeoutException();
                break;
            }
        }

        #endregion

        protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel.Id);
		}
	}
}