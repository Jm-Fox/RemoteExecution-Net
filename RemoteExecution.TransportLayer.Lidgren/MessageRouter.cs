using System;
using Lidgren.Network;

namespace RemoteExecution
{
    /// <summary>
    /// Routes Lidgren net*messages.
    /// </summary>
	public class MessageRouter
    {
        /// <summary>
        /// Event fired when the net connection is closed.
        /// </summary>
        public event Action<NetConnection> ConnectionClosed;
        /// <summary>
        /// Event fired when the net connection is opened.
        /// </summary>
        public event Action<NetConnection> ConnectionOpened;
        /// <summary>
        /// Action taken when data is received.
        /// </summary>
        public event Action<NetIncomingMessage> DataReceived;

        /// <summary>
        /// Method to be called when a net incoming message is received and ready for routing.
        /// </summary>
        public void Route(NetIncomingMessage msg)
		{
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.Data:
					HandleIncomingData(msg);
					break;
				case NetIncomingMessageType.StatusChanged:
					HandleStatusChange(msg);
					break;
			}
		}

		private void HandleClosedConnection(NetConnection connection)
		{
			if (ConnectionClosed != null)
				ConnectionClosed(connection);
		}

		private void HandleIncomingData(NetIncomingMessage msg)
		{
			if (DataReceived != null)
				DataReceived(msg);
		}

		private void HandleNewConnection(NetConnection connection)
		{
			if (ConnectionOpened != null)
				ConnectionOpened(connection);
		}

		private void HandleStatusChange(NetIncomingMessage msg)
		{
			switch ((NetConnectionStatus)msg.ReadByte())
			{
				case NetConnectionStatus.Connected:
					HandleNewConnection(msg.SenderConnection);
					break;
				case NetConnectionStatus.Disconnected:
					HandleClosedConnection(msg.SenderConnection);
					break;
			}
		}
	}
}