using RemoteExecution;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace EncryptedServices.Contracts
{
    public static class EncryptedConfigurator
    {
        /// <summary>
        /// Configures remote execution framework.
        /// Registers all supported transport layer providers.
        /// </summary>
        public static void Configure(ILidgrenCryptoProviderResolver providerResolver)
        {
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new LidgrenProvider(providerResolver));

            InterfaceResolver.Singleton.RegisterInterface(typeof(IAuthenticator));
            InterfaceResolver.Singleton.RegisterInterface(typeof(ISessionEncryptedCallback));
        }
    }
}
