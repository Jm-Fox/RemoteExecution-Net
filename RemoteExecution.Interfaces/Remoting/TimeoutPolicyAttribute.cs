using System;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Provides a non-default timeout for method invocations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class TimeoutPolicyAttribute : Attribute
    {
        /// <summary>
        /// Duration before a method invocation will timeout.
        /// </summary>
        public TimeSpan Timeout { get; }
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public TimeoutPolicyAttribute(double seconds)
        {
            Timeout = TimeSpan.FromSeconds(seconds);
        }
    }
}
