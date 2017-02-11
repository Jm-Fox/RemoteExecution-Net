using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Duplex channel class allowing to send and receive messages.
	/// </summary>
	public abstract class DuplexChannel : OutputChannel, IDuplexChannel
	{
        /// <summary>
		/// Fires when new message has been received through this channel.
        /// </summary>
	    public event Action<IMessage> Received;

	    /// <summary>
		/// Channel constructor.
		/// </summary>
		/// <param name="serializer">Serializer used to serialize message before send and deserialize it after receive.</param>
		protected DuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

        /// <summary>
        /// Method that should be called by implementation when message is received.
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnReceive(IMessage message)
		{
			if (Received != null)
				Received(message);
		}

        /// <summary>
        /// Deserializes a message from a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		protected IMessage DeserializeMessage(byte[] data)
		{
			var message = Serializer.Deserialize(data);
			var request = message as IRequestMessage;
			if (request != null)
				request.Channel = this;
			return message;
		}
	}
}