using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Serializers;

namespace RemoteExecution.TransportLayer.Lidgren.Wrappers.IT.MS
{
    [TestClass]
    public class LidgrenClientChannelWrapperTests
    {
        private const string AppId = "*";
        private const string Host = "127.0.0.1";
        private const ushort Port = 3231;
        private readonly string Url = $"net://{Host}:{Port}/{AppId}";

        private class LidgrenClientChannelFacade : LidgrenClientChannelWrapper
        {
            public LidgrenClientChannelFacade(LidgrenClientChannel inner) : base(inner)
            {
            }
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Configurator.Configure();
        }

        private static ClientConnection MakeFacadeConnection(string appId, string host, ushort port)
        {
            return new ClientConnection(new LidgrenClientChannelFacade(new LidgrenClientChannel(appId, host, port,
                new BinaryMessageSerializer(),
                new UnencryptedCryptoProviderResolver())), new OperationDispatcher(), new ConnectionConfig());
        }

        // todo: write more tests

        [TestMethod]
        public void ValidateBasicMethodCall()
        {
            IOperationDispatcher serverDisaptcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());
            using (StatelessServerEndpoint server = new StatelessServerEndpoint(Url, serverDisaptcher))
            {
                server.Start();
                using (ClientConnection client = MakeFacadeConnection(AppId, Host, Port))
                {
                    client.Open();
                    ICalculator calculator = client.RemoteExecutor.Create<ICalculator>();
                    Assert.AreEqual(4, calculator.Add(2, 2));
                }
            }
        }
    }
}
