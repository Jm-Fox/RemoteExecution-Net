using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;

namespace RemoteExecution.Config
{
	/// <summary>
	/// Connection configuration interface allowing to configure connection dependencies.
	/// </summary>
	public interface IConnectionConfig
	{
		/// <summary>
		/// Returns remote executor factory that should be used by connection.
		/// </summary>
		IRemoteExecutorFactory RemoteExecutorFactory { get; }

		/// <summary>
		/// Returns task scheduler that should be used by connection.
		/// </summary>
		ITaskScheduler TaskScheduler { get; }

        /// <summary>
        /// Returns the message factory that should be used.
        /// </summary>
        IMessageFactory MessageFactory { get; }

        /// <summary>
        /// Returns the message serializer that should be used
        /// </summary>
        IMessageSerializer MessageSerializer { get; }
    }
}