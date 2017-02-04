using AopAlliance.Intercept;
using RemoteExecution.Channels;
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

		public TwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher messageDispatcher, IMessageFactory messageFactory, string interfaceName)
		{
			_channel = channel;
			_messageDispatcher = messageDispatcher;
			_interfaceName = interfaceName;
		    _messageFactory = messageFactory;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();

			_messageDispatcher.Register(handler);
			try
			{
				_channel.Send(_messageFactory.CreateRequestMessage(handler.HandledMessageType, _interfaceName, invocation.Method.Name, invocation.Arguments, true));
				handler.WaitForResponse();
			}
			finally
			{
				_messageDispatcher.Unregister(handler.HandledMessageType);
			}
			return handler.GetValue();
		}

		#endregion

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel.Id);
		}
	}
}