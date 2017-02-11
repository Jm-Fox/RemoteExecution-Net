using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Endpoints;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;

namespace RemoteExecution.TransportLayer.Lidgren.Wrappers.IT.MS
{
    [TestClass]
    public class DurableLidgrenClientChannelTests
    {
        private const string Host = "127.0.0.1";
        private const ushort Port = 3231;
        private const ushort Port2 = 3232;
        private const string AppId = "*";
        private readonly string url = $"net://{Host}:{Port}/{AppId}";
        private readonly string url2 = $"net://{Host}:{Port2}/{AppId}";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new DurableLidgrenProvider());
        }

        [TestMethod]
        public void ValidateDurableSilentFailover()
        {
            IOperationDispatcher serverDispatcher = new OperationDispatcher();
            StatelessServerEndpoint server = new StatelessServerEndpoint(url, serverDispatcher);
            serverDispatcher.RegisterHandler<IWeakCalculator>(new WeakCalculator {Destroyed = () => server.Dispose()});
            server.Start();
            using (ClientConnection client = new ClientConnection(url))
            {
                client.Open();
                IWeakCalculator calculator = client.RemoteExecutor.Create<IWeakCalculator>();
                Assert.AreEqual(4, calculator.Add(2, 2));
                calculator.Destroy();
                server = new StatelessServerEndpoint(url, serverDispatcher);
                server.Start();
                Assert.AreEqual(11, calculator.Add(5, 6));
                server.Dispose();
            }
        }

        [TestMethod]
        public void ValidateDurableFailoverToNewEndPoint()
        {
            IOperationDispatcher serverDispatcher = new OperationDispatcher();
            StatelessServerEndpoint server = new StatelessServerEndpoint(url, serverDispatcher);
            serverDispatcher.RegisterHandler<IWeakCalculator>(new WeakCalculator { Destroyed = () => server.Dispose() });
            server.Start();
            using (DurableClientConnection client = new DurableClientConnection(url))
            {
                client.HandleClosedConnectionResponse = response => response.ReconnectPort = Port2;
                client.Open();
                IWeakCalculator calculator = client.RemoteExecutor.Create<IWeakCalculator>();
                Assert.AreEqual(4, calculator.Add(2, 2));
                calculator.Destroy();
                server = new StatelessServerEndpoint(url2, serverDispatcher);
                server.Start();
                Assert.AreEqual(11, calculator.Add(5, 6));
                server.Dispose();
            }
        }

        [TestMethod]
        public void ValidateDurableAbortReconnection()
        {
            IOperationDispatcher serverDispatcher = new OperationDispatcher();
            StatelessServerEndpoint server = new StatelessServerEndpoint(url, serverDispatcher);
            serverDispatcher.RegisterHandler<IWeakCalculator>(new WeakCalculator { Destroyed = () => server.Dispose() });
            server.Start();
            using (DurableClientConnection client = new DurableClientConnection(url))
            {
                client.HandleClosedConnectionResponse = response => response.Abort = true;
                client.Open();
                IWeakCalculator calculator = client.RemoteExecutor.Create<IWeakCalculator>();
                Assert.AreEqual(4, calculator.Add(2, 2));
                calculator.Destroy();
                server = new StatelessServerEndpoint(url2, serverDispatcher);
                server.Start();
                bool exceptionThrown = false;
                try
                {
                    calculator.Add(5, 6);
                }
                catch (NotConnectedException ex)
                {
                    exceptionThrown = ex.Message == "Network connection is not opened.";
                }
                Assert.IsTrue(exceptionThrown);
                server.Dispose();
            }
        }
    }
}