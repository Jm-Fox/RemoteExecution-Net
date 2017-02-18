using System;
using RemoteExecution;
using RemoteExecution.Config;
using RemoteExecution.Executors;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.Schedulers;
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
            DefaultConfig.Timeout = new TimeSpan(0, 0, 15);
            DefaultConfig.MessageSerializer = new ProtobufSerializer();
            DefaultConfig.MessageFactory = new ProtobufMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new DurableLidgrenProvider());
            InterfaceResolver.Singleton.RegisterInterface<ICalculator>();
        }
    }
}
