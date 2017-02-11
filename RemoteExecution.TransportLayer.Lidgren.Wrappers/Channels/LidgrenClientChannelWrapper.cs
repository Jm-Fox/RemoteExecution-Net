using Lidgren.Network;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
    /// <summary>
    /// Wraps a LidgrenClientChannel to allow use of the decorator pattern.
    /// </summary>
    public class LidgrenClientChannelWrapper : LidgrenClientChannel
    {
        #region Wrapped properties

        /// <summary>
        /// Lidgren net client.
        /// </summary>
        public override NetPeer Client
        {
            get { return Inner.Client; }
            set { Inner.Client = value; }
        }

        /// <summary>
        /// Lidgren net connection associated to channel.
        /// </summary>
        public override NetConnection Connection
        {
            get { return Inner.Connection; }
            set { Inner.Connection = value; }
        }

        /// <summary>
        /// Host address of connection.
        /// </summary>
        public override string Host
        {
            get { return Inner.Host; }
            set { Inner.Host = value; }
        }

        /// <summary>
        /// Returns true if channel is opened, otherwise false.
        /// </summary>
        public override bool IsOpen => Inner.IsOpen;

        /// <summary>
        /// Message loop.
        /// </summary>
        public override MessageLoop MessageLoop => Inner.MessageLoop;

        /// <summary>
        /// Message router.
        /// </summary>
        public override MessageRouter MessageRouter => Inner.MessageRouter;

        /// <summary>
        /// Host port.
        /// </summary>
        public override ushort Port
        {
            get { return Inner.Port; }
            set { Inner.Port = value; }
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        public override IMessageSerializer Serializer => Inner.Serializer;

        #endregion

        #region New members

        /// <summary>
        /// Actual LidgrenClientChannel
        /// </summary>
        protected readonly LidgrenClientChannel Inner;

        /// <summary>
        /// Wraps a LidgrenClientChannel.
        /// </summary>
        /// <param name="inner"></param>
        protected LidgrenClientChannelWrapper(LidgrenClientChannel inner)
            : base("**", null, 0, null, null)
        {
            Inner = inner;
            Inner.Closed += OnConnectionClose;
            Inner.Received += OnReceive;
        }

        #endregion

        #region Wrapped methods

        /// <summary>
        /// Closes channel. 
        /// It should not throw if channel is already closed.
        /// </summary>
        public override void Close()
        {
            Inner.Close();
        }

        /// <summary>
        /// Handles incoming message in lidgren format.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        public override void HandleIncomingMessage(NetIncomingMessage message)
        {
            Inner.HandleIncomingMessage(message);
        }

        /// <summary>
        /// Opens channel for sending and receiving messages.
        /// If channel has been closed, this method reopens it.
        /// </summary>
        public override void Open()
        {
            Inner.Open();
        }

        /// <summary>
        /// Sends given message through this channel.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public override void Send(IMessage message)
        {
            Inner.Send(message);
        }

        /// <summary>
        /// Sends data through channel.
        /// </summary>
        /// <param name="data">Data to send.</param>
        public override void SendData(byte[] data)
        {
            Inner.SendData(data);
        }

        #endregion
    }
}