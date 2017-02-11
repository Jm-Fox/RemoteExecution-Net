using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    [TestClass]
    public class IpEndPointProvisionTests
    {
        private IServerConnectionListener connectionListener;
        private readonly string applicationId = Guid.NewGuid().ToString();
        private IServerEndpoint server;
        private IOperationDispatcher dispatcher;
        private const string Host = "localhost";
        private const string ListenAddress = "0.0.0.0";
        private const ushort Port = 3231;

        private ClientConnection CreateClientConnection()
        {
            return new ClientConnection(new LidgrenClientChannel(applicationId, Host, Port, new BinaryMessageSerializer(), new UnencryptedCryptoProviderResolver()), new OperationDispatcher(), new ConnectionConfig());
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Configurator.Configure();
            InterfaceResolver.Singleton.RegisterInterface(typeof(Server.IEndPointProvisioner));
        }

        [TestMethod]
        public void ValidateIpEndPointProvision()
        {
            connectionListener = new LidgrenServerConnectionListener(applicationId, ListenAddress, Port, new BinaryMessageSerializer(), new UnencryptedCryptoProviderResolver());
            dispatcher = new OperationDispatcher();
            IPEndPoint senderEndPoint = null;
            IPEndPoint clientEndPoint = null;
            dispatcher.RegisterHandler<Server.IEndPointProvisioner>(new Server.EndPointProvisioner { OnStuff = e => senderEndPoint = e });
               server = new GenericServerEndpoint(connectionListener, new ServerConfig(), () => dispatcher);
            server.Start();

            using (ClientConnection client = CreateClientConnection())
            {
                client.Open();
                clientEndPoint = client.GetClientEndpoint();
                client.RemoteExecutor.Create<Client.IEndPointProvisioner>().DoStuff();
            }
            server.Dispose();
            Assert.AreEqual(clientEndPoint, senderEndPoint);
        }
    }
}
