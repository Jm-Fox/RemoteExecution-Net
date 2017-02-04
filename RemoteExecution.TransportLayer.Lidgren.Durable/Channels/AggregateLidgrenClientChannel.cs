using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
    /// <summary>
    /// Allows multiple channels to share one NetClient (and most importantly, one port)
    /// </summary>
    public class AggregateLidgrenClientChannel : LidgrenDuplexChannel, IClientChannel {
        /// <summary>
        /// Active Lidgren client
        /// </summary>
        protected static NetClient Client;
        /// <summary>
        /// Host address of connection
        /// </summary>
        protected string Host;
        private readonly MessageRouter _messageRouter;
        /// <summary>
        /// Host port of connection
        /// </summary>
        protected ushort Port;
        private MessageLoop _messageLoop;

        /// <summary>
        /// Creates client channel instance.
        /// </summary>
        /// <param name="applicationId">Application id that has to match to one used by <see cref="LidgrenServerConnectionListener"/>.</param>
        /// <param name="host">Host to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        /// <param name="serializer">Message serializer.</param>
        public AggregateLidgrenClientChannel(string applicationId, string host, ushort port, IMessageSerializer serializer)
            : base(serializer)
        {
            Host = host;
            Port = port;
            if (Client == null)
            {
                Client = new NetClient(new NetPeerConfiguration(applicationId));
            }
            _messageRouter = new MessageRouter();
            _messageRouter.DataReceived += HandleIncomingMessage;
            _messageRouter.ConnectionClosed += c => OnConnectionClose();
        }

        #region IClientChannel Members

        /// <summary>
        /// Opens channel for sending and receiving messages.
        /// If channel has been closed, this method reopens it.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Open()
        {
            if (IsOpen)
                throw new InvalidOperationException("Channel already opened.");
            _messageLoop = new MessageLoop(Client, _messageRouter.Route);
            if (Client.Status == NetPeerStatus.NotRunning)
            {
                Client.Start();
            }
            Connection = Client.Connect(Host, Port);
            Connection.WaitForConnectionToOpen();
        }

        #endregion

        /// <summary>
        /// Closes channel. 
        /// It should not throw if channel is already closed.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void Close()
        {
            base.Close();
            if (Client.ConnectionsCount == 0)
            {
                Client.Shutdown("Client disposed");
                Client.WaitForClose();
            }

            if (_messageLoop != null)
                _messageLoop.Dispose();
            _messageLoop = null;
        }
    }
}