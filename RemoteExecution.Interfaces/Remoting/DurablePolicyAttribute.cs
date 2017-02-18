using System;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Provides durable configuration for an interface and/or its methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class DurablePolicyAttribute : Attribute
    {
        /// <summary>
        /// Indicates that a method will fail when the channel is interrupted, even if the channel is durable.
        /// This means that any and all services on the other end will receive the method call at most one time.
        /// </summary>
        public bool NoRetries { get; set; }

        /// <summary>
        /// Indicates that the timeout cannot be reset if the connection is interrupted and restored.
        /// </summary>
        public bool TimeoutIsStrict { get; set; }

        /// <summary>
        /// Simple constructor.
        /// </summary>
        public DurablePolicyAttribute()
            : this(false, false)
        {
        }

        /// <summary>
        /// Simple constructor.
        /// </summary>
        /// <param name="noRetries"></param>
        public DurablePolicyAttribute(bool noRetries)
            : this (noRetries, false)
        {
        }

        /// <summary>
        /// Simple constructor.
        /// </summary>
        /// <param name="noRetries"></param>
        /// <param name="timeoutIsStrict"></param>
        public DurablePolicyAttribute(bool noRetries, bool timeoutIsStrict)
        {
            NoRetries = noRetries;
            TimeoutIsStrict = timeoutIsStrict;
        }
    }
}
