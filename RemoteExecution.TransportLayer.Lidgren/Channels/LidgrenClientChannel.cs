using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Config;
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
        private NetPeer _client;
        private MessageRouter _messageRouter;
        private MessageLoop _messageLoop;
        private string _host;
        private ushort _port;

        /// <summary>
        /// Lidgren net client.
        /// </summary>
        public virtual NetPeer Client
        {
            get { return _client; }
            set { _client = value; }
        }

        /// <summary>
        /// Host address of connection.
        /// </summary>
        public virtual string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Message router.
        /// </summary>
        public virtual MessageRouter MessageRouter => _messageRouter;
        /// <summary>
        /// Host port.
        /// </summary>
        public virtual ushort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Message loop.
        /// </summary>
        public virtual MessageLoop MessageLoop => _messageLoop;

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
			_host = host;
			_port = port;
		    _client =
		        new NetClient(new NetPeerConfiguration(applicationId)
		        {
		            ConnectionTimeout = (float)DefaultConfig.DefaultTimeout.TotalSeconds
		        });
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
		public virtual void Open()
		{
			if (IsOpen)
				throw new InvalidOperationException("Channel already opened.");
			_messageLoop = new MessageLoop(Client, MessageRouter.Route);
			Client.Start();
			Connection = Client.Connect(Host, Port);
			Connection.WaitForConnectionToOpen();
		}

		#endregion

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
		public override void Close()
		{
			base.Close();
		    if (Client.Status == NetPeerStatus.Running || Client.Status == NetPeerStatus.Starting)
		    {
		        Client.Shutdown("Client disposed");
		        Client.WaitForClose();
		    }

		    if (MessageLoop != null)
				MessageLoop.Dispose();
			_messageLoop = null;
		}
    }
}