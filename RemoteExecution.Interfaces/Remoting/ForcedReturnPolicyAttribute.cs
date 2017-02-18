using System;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Defines a non-default return policy for a method or interface and all of its methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class ForcedReturnPolicyAttribute : Attribute
    {
        /// <summary>
        /// Forced return policy.
        /// </summary>
        public ReturnPolicy ForcedReturnPolicy { get; set; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public ForcedReturnPolicyAttribute()
            : this (ReturnPolicy.TwoWay)
        {
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="policy"></param>
        public ForcedReturnPolicyAttribute(ReturnPolicy policy)
        {
            ForcedReturnPolicy = policy;
        }
    }
}
