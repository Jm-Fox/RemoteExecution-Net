using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lidgren.Network;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Lidgren duplex channel class allowing to send and receive messages.
	/// </summary>
	public class LidgrenDuplexChannel : DuplexChannel
	{
		private static readonly IEnumerable<NetConnectionStatus> _validConnectionStatus = new[] { NetConnectionStatus.Connected, NetConnectionStatus.RespondedConnect };

        /// <summary>
        /// Designated encryption provider
        /// </summary>
        protected readonly ILidgrenCryptoProvider CryptoProvider;

        /// <summary>
        /// Returns true if channel is opened, otherwise false.
        /// </summary>
        public override bool IsOpen
		{
			get { return Connection != null && _validConnectionStatus.Contains(Connection.Status); }
		}

		/// <summary>
		/// Lidgren net connection associated to channel.
		/// </summary>
		protected NetConnection Connection { get; set; }

		/// <summary>
		/// Creates channel instance with specified message serializer.
		/// </summary>
		/// <param name="serializer">Message serializer.</param>
		protected LidgrenDuplexChannel(IMessageSerializer serializer)
			: this(serializer, new UnencryptedCryptoProvider())
		{
		}

		/// <summary>
		/// Creates channel instance with specified net connection and message serializer.
		/// </summary>
		/// <param name="connection">Lidgren net connection.</param>
		/// <param name="serializer">Message serializer.</param>
		public LidgrenDuplexChannel(NetConnection connection, IMessageSerializer serializer)
			: this(serializer, new UnencryptedCryptoProvider())
		{
			Connection = connection;
        }

        /// <summary>
        /// Creates channel instance with specified message serializer.
        /// </summary>
        /// <param name="serializer">Message serializer.</param>
        /// <param name="cryptoProvider">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s</param>
        protected LidgrenDuplexChannel(IMessageSerializer serializer, ILidgrenCryptoProvider cryptoProvider)
            : base(serializer)
        {
            CryptoProvider = cryptoProvider;
        }

        /// <summary>
        /// Creates channel instance with specified net connection and message serializer.
        /// </summary>
        /// <param name="connection">Lidgren net connection.</param>
        /// <param name="serializer">Message serializer.</param>
        /// <param name="cryptoProvider">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s</param>
        public LidgrenDuplexChannel(NetConnection connection, IMessageSerializer serializer, ILidgrenCryptoProvider cryptoProvider)
            : this(serializer, cryptoProvider)
        {
            Connection = connection;
        }

        // Some methods might need the sender's IP. In this case, the contract will differ between the client and the server.
        // This method allows the following two methods to match:
        // Client.ICalculator: { int Add_(int x, int y); }
        // Server.ICalculator: { int Add_(int x, int y, IPEndPoint clientAddress);}
	    private static bool SenderEndPointIsExpectedByInterface(IRequestMessage request)
	    {
            // This can be more flexible, but for something that executes for every request, speed is best.
            // If a developer wants more flexibility, they can change this method to come from an attribute
            // on the method or something else that's friendlier
	        string name = request.MethodName;
            return  name[name.Length - 1] == '_';
	    }

	    /// <summary>
        /// Handles incoming message in lidgren format.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        public void HandleIncomingMessage(NetIncomingMessage message)
		{
            CryptoProvider.Resolve(message.SenderEndPoint)?.Decrypt(message);
		    byte[] bytes = message.ReadBytes(message.LengthBytes);
		    IMessage imessage = DeserializeMessage(bytes);
            IRequestMessage request = imessage as IRequestMessage;
		    if (request != null && SenderEndPointIsExpectedByInterface(request)) {
                // Modified such that interface method names that end in _ will provide the Client's IP
                object[] args2 = new object[request.Args.Length + 1];
                request.Args.CopyTo(args2, 0);
		        args2[args2.Length - 1] = message.SenderEndPoint;
		    }
		    OnReceive(imessage);
		}

		/// <summary>
		/// Handles lidgren connection close event (fires Closed event).
		/// </summary>
		public virtual void OnConnectionClose()
		{
			FireChannelClosed();
		}

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
		protected override void Close()
		{
			if (Connection != null && IsOpen)
				Connection.Disconnect("Channel closed");
		}

		/// <summary>
		/// Sends data through channel.
		/// </summary>
		/// <param name="data">Data to send.</param>
		protected override void SendData(byte[] data)
		{
			if (!IsOpen)
				throw new NotConnectedException("Network connection is not opened.");
            Connection.Peer.SendMessage(CreateOutgoingMessage(data), Connection, NetDeliveryMethod.ReliableUnordered, 0);
		}

	    /// <summary>
        /// Creates a Lidgren message from a byte array
        /// </summary>
        /// <param name="data">Bytes to send</param>
        /// <returns>Lidgren outgoing message</returns>
        protected NetOutgoingMessage CreateOutgoingMessage(byte[] data)
		{
			var msg = Connection.Peer.CreateMessage(data.Length);
			msg.Write(data);
            CryptoProvider.Resolve(Connection.RemoteEndPoint)?.Encrypt(msg);
			return msg;
		}
	}
}
