using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Remoting;
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
        public static TimeSpan Timeout { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Default timeout-is-strict. See also <see cref="DurablePolicyAttribute"/>
        /// </summary>
        public static bool TimeoutIsStrict { get; set; } = false;

        /// <summary>
        /// Default no-retries. See also <see cref="DurablePolicyAttribute"/>
        /// </summary>
        public static bool NoRetries { get; set; } = false;

        /// <summary>
        /// Default no-retries. See also <see cref="IpEndPointPolicyAttribute"/>
        /// </summary>
        public static bool RequiresIpEndpoint { get; set; } = false;
    }
}