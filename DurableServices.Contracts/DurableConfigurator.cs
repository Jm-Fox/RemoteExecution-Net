using System;
using RemoteExecution;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace DurableServices.Contracts
{
    public static class DurableConfigurator
    {
        /// <summary>
        /// Configures remote execution framework.
        /// Registers all supported transport layer providers.
        /// </summary>
        public static void Configure()
        {
            DefaultConfig.DefaultTimeout = new TimeSpan(0, 0, 15);
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new DurableLidgrenProvider());
        }
    }
}
