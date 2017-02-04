using System;
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

        /// <summary>
        /// Quick implementation of SenderEndPointIsExpectedByInterface
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // Some methods might need the sender's IP. In this case, the contract will differ between the client and the server.
        // This method allows the following two methods to match:
        // Client.ICalculator: { int Add_(int x, int y); }
        // Server.ICalculator: { int Add_(int x, int y, IPEndPoint clientAddress);}
        public static bool QuickSenderEndPointIsExpectedByInterface(IRequestMessage request)
	    {
	        string name = request.MethodName;
            return  name[name.Length - 1] == '_';
        }

        /// <summary>
        /// Slow implementation of SenderEndPointIsExpectedByInterface.
        /// This method is INCOMPLETE. You must implement and register your own <see cref="InterfaceResolver"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // Some methods might need the sender's IP. In this case, the contract will differ between the client and the server.
        // This method allows the following two methods to match:
        // Client.ICalculator: { int Add(int x, int y); }
        //                           [RequiresIpEndPoint]
        // Server.ICalculator: { int Add(int x, int y, IPEndPoint clientAddress);}
        // [RequiresIpEndPoint] can be given to the client, but it doesn't make any difference. The interfaces will already be
        // different by nature of the parameters, so why bother?
        public static bool SlowSenderEndPointIsExpectedByInterface(IRequestMessage request)
        {
            return RequiresIpEndPointAttribute.RequiresIpEndPoint(
                InterfaceResolver.Singleton.GetInterface(request.MessageType), request.MethodName);
        }

        /// <summary>
        /// Indicates that an interface's method expects an IPEndPoint for the sender, but the sender's contract doesn't specify it.
        /// </summary>
        public static Func<IRequestMessage, bool> SenderEndPointIsExpectedByInterface =
	        QuickSenderEndPointIsExpectedByInterface;

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
