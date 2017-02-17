using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;

namespace RemoteExecution.Config
{
    /// <summary>
    /// Connection configuration class allowing to configure connection dependencies.
    /// </summary>
    public class ConnectionConfig : IConnectionConfig
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionConfig()
        {
            RemoteExecutorFactory = DefaultConfig.RemoteExecutorFactory;
            TaskScheduler = DefaultConfig.TaskScheduler;
        }

        #region IConnectionConfig Members

        /// <summary>
        /// Returns remote executor factory that should be used by connection.
        /// Default value is taken from <see cref="DefaultConfig"/>.
        /// </summary>
        public IRemoteExecutorFactory RemoteExecutorFactory { get; set; }

        /// <summary>
        /// Returns task scheduler that should be used by connection.
        /// Default value is taken from <see cref="DefaultConfig"/>.
        /// </summary>
        public ITaskScheduler TaskScheduler { get; set; }

        /// <summary>
        /// Returns the message factory that should be used.
        /// </summary>
        public IMessageFactory MessageFactory { get; set; }

        /// <summary>
        /// Returns the message serializer that should be used.
        /// </summary>
        public IMessageSerializer MessageSerializer { get; set; }

        #endregion
        }
}