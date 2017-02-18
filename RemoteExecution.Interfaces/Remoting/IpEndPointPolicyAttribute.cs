using System;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Indicates that an contract method (or all of an interface's methods, unless otherwise specified)
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class IpEndPointPolicyAttribute : Attribute
    {
        /// <summary>
        /// Whether or not a method requries an IpEndPoint.
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Default constructor sets Required = true.
        /// </summary>
        public IpEndPointPolicyAttribute()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes Required.
        /// </summary>
        /// <param name="required"></param>
        public IpEndPointPolicyAttribute(bool required)
        {
            Required = required;
        }
    }
}
