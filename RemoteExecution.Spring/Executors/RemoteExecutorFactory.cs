using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Executors
{
	/// <summary>
	/// Remote executor factory class allowing to create remote executors.
	/// </summary>
	public class RemoteExecutorFactory : IRemoteExecutorFactory
	{
	    private readonly IMessageFactory _messageFactory;

        /// <summary>
        /// Creates a remote executor factory using the default message factory.
        /// </summary>
	    public RemoteExecutorFactory()
            : this (DefaultConfig.MessageFactory)
	    {
        }

        /// <summary>
        /// Creates a remote executor factory using the specified message factory.
        /// </summary>
        /// <param name="messageFactory"></param>
        public RemoteExecutorFactory(IMessageFactory messageFactory)
        {
            _messageFactory = messageFactory;
        }

        #region IRemoteExecutorFactory Members

        /// <summary>
        /// Creates remote executor for given channel, using given message dispatcher for receiving operation responses.
        /// </summary>
        /// <param name="channel">Duplex channel.</param>
        /// <param name="dispatcher">Message dispatcher used for receiving operation responses.</param>
        /// <returns>Executor.</returns>
        public IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher)
		{
			return new RemoteExecutor(channel, dispatcher, _messageFactory);
		}

		/// <summary>
		/// Creates broadcast remote executor for given broadcast channel.
		/// </summary>
		/// <param name="channel">Broadcast channel.</param>
		/// <returns>Broadcast executor.</returns>
		public IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel)
		{
			return new BroadcastRemoteExecutor(channel);
		}

		#endregion
	}
}