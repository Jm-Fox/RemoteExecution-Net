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
using RemoteExecution.TransportLayer.Lidgren.IT.MS;

namespace RemoteExecution.TransportLayer.Lidgren.Wrappers.IT.MS
{
    [TestClass]
    public class SharedLidgrenClientChannelTests
    {
        private const string Host = "127.0.0.1";
        private const ushort Port = 3231;
        private const ushort Port2 = 3232;
        private const ushort Port3 = 3233;
        private const string AppId = "*";
        private readonly string url = $"net://{Host}:{Port}/{AppId}";
        private readonly string url2 = $"net://{Host}:{Port2}/{AppId}";
        private readonly string url3 = $"net://{Host}:{Port3}/{AppId}";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new SharedLidgrenProvider());
        }

        private static ClientConnection CreateClientConnection(string clientId, ushort port, IOperationDispatcher dispatcher)
        {
            return new ClientConnection($"net://{Host}:{port}/{AppId}", clientId, dispatcher);
        }

        [TestMethod]
        public void ValidateSharedPorts()
        {
            // Arrange
            IOperationDispatcher serverDisaptcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());

            IOperationDispatcher clienDispatcher = new OperationDispatcher();
            
            // Act
            using (StatelessServerEndpoint server1 = new StatelessServerEndpoint(url, serverDisaptcher))
            using (StatelessServerEndpoint server2 = new StatelessServerEndpoint(url2, serverDisaptcher))
            {
                using (ClientConnection c1 = CreateClientConnection("a", Port, clienDispatcher))
                using (ClientConnection c2 = CreateClientConnection("a", Port2, clienDispatcher))
                {
                    server1.Start();
                    server2.Start();
                    c1.Open();
                    c2.Open();

                    // Assert
                    Assert.AreEqual(c1.GetClientEndpoint(), c2.GetClientEndpoint());
                }
            }
        }

        [TestMethod]
        public void ValidateSharedActuallyWorks()
        {
            // Arrange
            IOperationDispatcher serverDispatcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());

            IOperationDispatcher clienDispatcher = new OperationDispatcher();

            // Act
            using (StatelessServerEndpoint server2 = new StatelessServerEndpoint(url2, serverDispatcher))
            using (StatelessServerEndpoint server3 = new StatelessServerEndpoint(url3, serverDispatcher))
            {
                using (ClientConnection c1 = CreateClientConnection("a", Port2, clienDispatcher))
                using (ClientConnection c2 = CreateClientConnection("a", Port3, clienDispatcher))
                {
                    server2.Start();
                    server3.Start();
                    c1.Open();
                    c2.Open();

                    // Assert
                    Assert.AreEqual(4, c1.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                    Assert.AreEqual(4, c2.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                }
            }
        }

        [TestMethod]
        public void ValidateSingle()
        {
            // Arrange
            IOperationDispatcher dispatcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());

            // Act
            using (StatelessServerEndpoint server1 = new StatelessServerEndpoint(url, dispatcher))
            {
                using (ClientConnection c1 = CreateClientConnection("a", Port, new OperationDispatcher()))
                {
                    server1.Start();
                    c1.Open();
                    
                    Assert.AreEqual(4, c1.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                }
            }
        }

        [TestMethod]
        public void ValidateCloseShared()
        {
            // Arrange
            bool exceptionThrown = false;
            IOperationDispatcher serverDispatcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());

            IOperationDispatcher clienDispatcher = new OperationDispatcher();

            // Act
            using (StatelessServerEndpoint server1 = new StatelessServerEndpoint(url2, serverDispatcher))
            using (StatelessServerEndpoint server2 = new StatelessServerEndpoint(url3, serverDispatcher))
            {
                using (ClientConnection c1 = CreateClientConnection("a", Port2, clienDispatcher))
                using (ClientConnection c2 = CreateClientConnection("a", Port3, clienDispatcher))
                {
                    server1.Start();
                    server2.Start();
                    c1.Open();
                    c2.Open();
                    
                    c1.Dispose();
                    try
                    {
                        c2.RemoteExecutor.Create<ICalculator>().Add(2, 2);
                    }
                    catch (NotConnectedException)
                    {
                        exceptionThrown = true;
                    }
                }
            }
            // Assert
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void ValidateStartWithDistinct()
        {
            // Arrange
            IOperationDispatcher dispatcher = new OperationDispatcher()
                .RegisterHandler<ICalculator>(new Calculator());

            // Act
            using (StatelessServerEndpoint server = new StatelessServerEndpoint(url, dispatcher))
            using (StatelessServerEndpoint server2 = new StatelessServerEndpoint(url2, dispatcher))
            using (StatelessServerEndpoint server3 = new StatelessServerEndpoint(url3, dispatcher))
            {
                using (ClientConnection c1 = new ClientConnection(url))
                using (ClientConnection c2 = CreateClientConnection("a", Port2, new OperationDispatcher()))
                using (ClientConnection c3 = CreateClientConnection("b", Port3, new OperationDispatcher()))
                {
                    server.Start();
                    server2.Start();
                    server3.Start();
                    c1.Open();
                    c2.Open();
                    c3.Open();

                    // Assert
                    Assert.AreEqual(4, c1.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                    Assert.AreEqual(4, c2.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                    Assert.AreEqual(4, c3.RemoteExecutor.Create<ICalculator>().Add(2, 2));
                    Assert.AreNotEqual(c1.GetClientEndpoint(), c2.GetClientEndpoint());
                    Assert.AreNotEqual(c2.GetClientEndpoint(), c3.GetClientEndpoint());
                }
            }
        }
    }
}