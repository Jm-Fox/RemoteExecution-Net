using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Executors
{
	internal class RemoteExecutor : IRemoteExecutor
	{
		private readonly IDuplexChannel _channel;
		private readonly IMessageDispatcher _dispatcher;
	    private readonly IMessageFactory _messageFactory;

        public RemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher, IMessageFactory messageFactory)
		{
			_channel = channel;
			_dispatcher = dispatcher;
		    _messageFactory = messageFactory;

		}

		#region IRemoteExecutor Members

		public T Create<T>()
		{
			return Create<T>(NoResultMethodExecution.TwoWay);
		}

		public T Create<T>(NoResultMethodExecution noResultMethodExcecution)
		{
			var remoteCallInterceptor = new RemoteCallInterceptor(
				new OneWayRemoteCallInterceptor(_channel, _messageFactory, typeof(T).Name),
				new TwoWayRemoteCallInterceptor(_channel, _dispatcher, _messageFactory, typeof(T).Name),
				noResultMethodExcecution);

			var factory = new ProxyFactory(typeof(T), remoteCallInterceptor);
			return (T)factory.GetProxy();
		}

		#endregion
	}
}