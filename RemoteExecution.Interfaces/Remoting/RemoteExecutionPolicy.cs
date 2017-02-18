using System;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Provides configuration for how a two-way method will be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class RemoteExecutionPolicy : Attribute
    {
        /// <summary>
        /// Timeout before remote method invocation is considered a failure.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Whether or not the method invocation can be retried if the connection is interrupted.
        /// </summary>
        public bool NoRetries { get; set; }

        /// <summary>
        /// Whether or not the timeout can be reset when the connection is interrupted and restored.
        /// </summary>
        public bool TimeoutIsStrict { get; set; }

        /// <summary>
        /// Whether or not the method invocation requires an IPEndPoint.
        /// </summary>
        public bool RequiresIpEndPoint { get; set; }

        /// <summary>
        /// Default no return policy.
        /// </summary>
        // todo: make internal set
        public ReturnPolicy DefaultReturnPolicy { get; set; }

        /// <summary>
        /// Forced return policy.
        /// </summary>
        public ReturnPolicy? ForcedReturnPolicy { get; set; }

        /// <summary>
        /// Actual return policy to be used.
        /// </summary>
        public ReturnPolicy ActualReturnPolicy => ForcedReturnPolicy ?? DefaultReturnPolicy;

        /// <summary>
        /// Creates a copy of this policy
        /// </summary>
        /// <returns>Member-wise copy</returns>
        public RemoteExecutionPolicy Clone()
        {
            return new RemoteExecutionPolicy
            {
                Timeout = Timeout,
                NoRetries = NoRetries,
                TimeoutIsStrict = TimeoutIsStrict,
                RequiresIpEndPoint = RequiresIpEndPoint,
                DefaultReturnPolicy = DefaultReturnPolicy,
                ForcedReturnPolicy = ForcedReturnPolicy
            };
        }
    }
}
