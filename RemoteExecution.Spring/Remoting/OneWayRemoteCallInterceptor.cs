using System;
using AopAlliance.Intercept;
using RemoteExecution.Channels;
using RemoteExecution.Config;

namespace RemoteExecution.Remoting
{
	internal class OneWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IOutputChannel _channel;
		private readonly string _interfaceName;

		public OneWayRemoteCallInterceptor(IOutputChannel channel, string interfaceName)
		{
			_channel = channel;
			_interfaceName = interfaceName;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
		    _channel.Send(DefaultConfig.MessageFactory.CreateRequestMessage(Guid.NewGuid().ToString(), _interfaceName,
		        invocation.Method.Name, invocation.Arguments, false));
			return null;
		}

		#endregion
	}
}