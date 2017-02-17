using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.Serializers;

namespace RemoteExecution.Config
{
    /// <summary>
    /// Default configuration used by both server and client classes.
    /// </summary>
    public static class DefaultConfig
    {
        /// <summary>
        /// Default remote executor factory.
        /// </summary>
        public static IRemoteExecutorFactory RemoteExecutorFactory { get; set; }

        /// <summary>
        /// Default task scheduler.
        /// </summary>
        public static ITaskScheduler TaskScheduler { get; set; }

        /// <summary>
        /// Default message factory.
        /// </summary>
        public static IMessageFactory MessageFactory { get; set; }

        /// <summary>
        /// Default message serializer.
        /// </summary>
        public static IMessageSerializer MessageSerializer { get; set; }

        /// <summary>
        /// Default two-way request timeout.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; } = new TimeSpan(0, 1, 0);
    }
}