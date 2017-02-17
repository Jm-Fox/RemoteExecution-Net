using System;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Provides a non-default timeout for method invocations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class TimeoutAttribute : Attribute
    {
        /// <summary>
        /// Duration before a method invocation will timeout.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="timeout"></param>
        public TimeoutAttribute(TimeSpan timeout)
        {
            Timeout = timeout;
        }
    }
}
