using RemoteExecution.Config;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	/// <summary>
	/// Remote execution framework configurator.
	/// </summary>
	public static class Configurator
	{
		/// <summary>
		/// Configures remote execution framework.
		/// Registers all supported transport layer providers.
		/// </summary>
		public static void Configure()
		{
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
		    DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new LidgrenProvider());
        }
	}
}
