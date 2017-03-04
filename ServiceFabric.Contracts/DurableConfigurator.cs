using System;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Schedulers;
using RemoteExecution.TransportLayer;
using RemoteExecution.Serializers;
using RemoteExecution.Executors;
using RemoteExecution;

namespace ServiceFabric.Contracts
{
    public static class DurableConfigurator
    {
        public static void Configure()
        {
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            DefaultConfig.Timeout = TimeSpan.FromSeconds(5);
            TransportLayerResolver.Register(new DurableLidgrenProvider());
        }
    }
}