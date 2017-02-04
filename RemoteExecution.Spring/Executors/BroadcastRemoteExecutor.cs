using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Executors
{
	internal class BroadcastRemoteExecutor : IBroadcastRemoteExecutor
	{
		private readonly IBroadcastChannel _broadcastChannel;
	    private readonly IMessageFactory _messageFactory;

		public BroadcastRemoteExecutor(IBroadcastChannel broadcastChannel, IMessageFactory messageFactory)
		{
			_broadcastChannel = broadcastChannel;
		    _messageFactory = messageFactory;
		}

		#region IBroadcastRemoteExecutor Members

		public T Create<T>()
		{
			var interfaceType = typeof(T);

			VerifyInterfaceMethods(interfaceType, interfaceType.Name);

			return (T)new ProxyFactory(interfaceType, new OneWayRemoteCallInterceptor(_broadcastChannel, _messageFactory, interfaceType.Name)).GetProxy();
		}

		#endregion

		private static void VerifyInterfaceMethods(Type interfaceType, string name)
		{
			if (interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.ReturnType != typeof(void)))
				throw new InvalidOperationException(string.Format("{0} interface cannot be used for broadcasting because some of its methods returns result.", name));

			foreach (var baseInterface in interfaceType.GetInterfaces())
				VerifyInterfaceMethods(baseInterface, name);
		}
	}
}