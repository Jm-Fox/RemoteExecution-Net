using System;
using AopAlliance.Intercept;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Remoting
{
	internal class OneWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IOutputChannel _channel;
		private readonly string _interfaceName;
	    private readonly IMessageFactory _messageFactory;
        
        public OneWayRemoteCallInterceptor(IOutputChannel channel, IMessageFactory messageFactory, string interfaceName)
		{
			_channel = channel;
			_interfaceName = interfaceName;
		    _messageFactory = messageFactory;

		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
		    _channel.Send(_messageFactory.CreateRequestMessage(Guid.NewGuid().ToString(), _interfaceName,
		        invocation.Method.Name, invocation.Arguments, false));
			return null;
		}

		#endregion
	}
}