using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
    /// <summary>
	/// Lidgren client channel allowing to send and receive messages. 
	/// Client channel has to be opened with Open() method before use
	/// and can be reopened after it has been closed.
	/// </summary>
	public class LidgrenClientChannel : LidgrenDuplexChannel, IClientChannel
	{
        /// <summary>
        /// Active Lidgren client
        /// </summary>
		protected readonly NetClient Client;
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
        /// <param name="cryptoProviderResolver">Crypto provider resolver.</param>
        public LidgrenClientChannel(string applicationId, string host, ushort port, IMessageSerializer serializer, ILidgrenCryptoProviderResolver cryptoProviderResolver)
			: base(serializer, cryptoProviderResolver)
		{
			Host = host;
			Port = port;
			Client = new NetClient(new NetPeerConfiguration(applicationId));
            // todo: remove debug timeout
            Client.Configuration.ConnectionTimeout = 10000f;
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
			Client.Start();
			Connection = Client.Connect(Host, Port);
			Connection.WaitForConnectionToOpen();
		}

		#endregion

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
		protected override void Close()
		{
			base.Close();
			Client.Shutdown("Client disposed");
			Client.WaitForClose();

			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}
	}
}