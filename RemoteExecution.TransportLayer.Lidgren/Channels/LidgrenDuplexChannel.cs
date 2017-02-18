using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lidgren.Network;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.InterfaceResolution;
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
        /// Designated encryption providerResolver
        /// </summary>
        protected readonly ILidgrenCryptoProviderResolver CryptoProviderResolver;

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
		public virtual NetConnection Connection { get; set; }

		/// <summary>
		/// Creates channel instance with specified message serializer.
		/// </summary>
		/// <param name="serializer">Message serializer.</param>
		protected LidgrenDuplexChannel(IMessageSerializer serializer)
			: this(serializer, new UnencryptedCryptoProviderResolver())
		{
        }

        /// <summary>
        /// Creates channel instance with specified message serializer.
        /// </summary>
        /// <param name="serializer">Message serializer.</param>
        /// <param name="cryptoProviderResolver">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s</param>
        public LidgrenDuplexChannel(IMessageSerializer serializer, ILidgrenCryptoProviderResolver cryptoProviderResolver)
            : base(serializer)
        {
            CryptoProviderResolver = cryptoProviderResolver;
        }

        /// <summary>
        /// Handles incoming message in lidgren format.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        public virtual void HandleIncomingMessage(NetIncomingMessage message)
        {
            IMessage imessage;
            try
		    {
		        CryptoProviderResolver.Resolve(message.SenderEndPoint)?.Decrypt(message);
		        imessage = DeserializeMessage(message.ReadBytes(message.LengthBytes));
		    }
		    catch (Exception)
		    {
                // If a message can't be deserialized / decrypted, it's fair to assume
                // it's either malicious or accidental, and so it should be ignored. We especially
                // don't want any malicious messages to raise exceptions that would crash the
                // message loop.
		        return;
		    }
		    IRequestMessage request = imessage as IRequestMessage;
		    if (request != null && InterfaceResolver.SenderEndPointIsExpectedByInterface(request)) {
                // Modified such that interface method names that end in _ will provide the Client's IP
                object[] args2 = new object[request.Args?.Length + 1 ?? 1];
                request.Args?.CopyTo(args2, 0);
		        args2[args2.Length - 1] = message.SenderEndPoint;
		        request.Args = args2;
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
		public override void Close()
		{
			if (Connection != null && IsOpen)
				Connection.Disconnect("Channel closed");
		}

        /// <summary>
        /// Sends data through channel.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>True if the connection is open, false if the connection is paused.</returns>
        public override bool SendData(byte[] data)
		{
			if (!IsOpen)
				throw new NotConnectedException("Network connection is not opened.");
            Connection.Peer.SendMessage(CreateOutgoingMessage(data), Connection, NetDeliveryMethod.ReliableUnordered, 0);
		    return true;
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
            CryptoProviderResolver.Resolve(Connection.RemoteEndPoint)?.Encrypt(msg);
			return msg;
		}
	}
}
