using RemoteExecution;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace SharedClientServices.Contracts
{
    public static class SharedConfigurator
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
            TransportLayerResolver.Register(new SharedLidgrenProvider());
            InterfaceResolver.Singleton.RegisterInterface(typeof(IBasicService));
        }
    }
}
