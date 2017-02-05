using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Endpoints;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;
using RemoteExecution.Executors;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    [TestClass]
    public class CryptoTests
    {
        private IServerConnectionListener _connectionListener;
        private readonly string _applicationId = Guid.NewGuid().ToString();
        private IOperationDispatcher _dispatcher;
        private const string _listenAddress = "0.0.0.0";
        private const string _host = "localhost";
        private const ushort _port = 3231;

        private IServerEndpoint CreateServer(ILidgrenCryptoProviderResolver providerResolver)
        {
            DefaultConfig.MessageSerializer = new BinaryMessageSerializer();
            DefaultConfig.MessageFactory = new DefaultMessageFactory();
            DefaultConfig.RemoteExecutorFactory = new RemoteExecutorFactory();
            DefaultConfig.TaskScheduler = new AsyncTaskScheduler();
            TransportLayerResolver.Register(new LidgrenProvider(providerResolver));
            _connectionListener = new LidgrenServerConnectionListener(_applicationId, _listenAddress, _port, new BinaryMessageSerializer(), providerResolver);

            _dispatcher = new OperationDispatcher();
            _dispatcher.RegisterHandler<ICalculator>(new Calculator());
            _dispatcher.RegisterHandler<IGreeter>(new Greeter());

            IServerEndpoint server = new GenericServerEndpoint(_connectionListener, new ServerConfig(), () => _dispatcher);
            server.Start();
            return server;
        }

        private ClientConnection CreateClientConnection(ILidgrenCryptoProviderResolver provider)
        {
            return new ClientConnection(new LidgrenClientChannel(_applicationId, _host, _port, new BinaryMessageSerializer(), provider), new OperationDispatcher(), new ConnectionConfig());
        }

        [TestMethod]
        public void TestAesCrypto()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, _port);
            BasicLidgrenCryptoProviderResolver resolver = new BasicLidgrenCryptoProviderResolver();
            ObservableAESEncryption encryption = new ObservableAESEncryption(_applicationId, "topsecret");
            int cryptoFired = 0;
            const int len = 1000;
            // 0: request before encrypt;            1: request after encrypt
            // 2: request received before decrypt;   3: request recieved after decrypt
            // 4: response before encrypt;           5: response after encrypt
            // 6: response received before decrypt;  7: response received after decrypt
            List<byte[]> serializedBytes = new List<byte[]>();
            encryption.OnEncrypt = (b, a) =>
            {
                serializedBytes.Add(b);
                serializedBytes.Add(a);
                cryptoFired++;
            };
            encryption.OnDecrypt = (b, a) =>
            {
                serializedBytes.Add(b);
                serializedBytes.Add(a);
                cryptoFired++;
            };
            using (CreateServer(resolver))
            using (ClientConnection client = CreateClientConnection(resolver))
            {
                client.Open();
                ICalculator calculator = client.RemoteExecutor.Create<ICalculator>();
                Assert.AreEqual(5, calculator.Add(3, 2));
                Assert.AreEqual(0, cryptoFired);
                IPEndPoint clientEndPoint = client.GetClientEndpoint();
                resolver.Register(clientEndPoint, encryption);
                resolver.Register(serverEndPoint, encryption);
                Assert.AreEqual(11, calculator.Add(5, 6));
                Assert.AreEqual(4, cryptoFired);
                CollectionAssert.AreNotEqual(serializedBytes[0], serializedBytes[1]);
                CollectionAssert.AreEqual(serializedBytes[1], serializedBytes[2]);
                CollectionAssert.AreNotEqual(serializedBytes[2], serializedBytes[3]);
                CollectionAssert.AreNotEqual(serializedBytes[4], serializedBytes[5]);
                CollectionAssert.AreEqual(serializedBytes[5], serializedBytes[6]);
                CollectionAssert.AreNotEqual(serializedBytes[6], serializedBytes[7]);
            }
        }

    }
}
