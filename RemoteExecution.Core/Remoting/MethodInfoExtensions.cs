using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Method info extensions.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Gets a single attribute on a type. If the child doesn't have the attribute, the parent is checked.
        /// </summary>
        /// <typeparam name="T">Attribute to look for.</typeparam>
        /// <param name="me">Type to check.</param>
        /// <returns>Attribute if found, otherwise null.</returns>
        public static T GetAttributeOrDefault<T>(this MethodInfo me) where T : Attribute
        {
            return me.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
        }

        /// <summary>
        /// Gets the Remote execution policy for a method.
        /// </summary>
        /// <param name="me">MethodInfo to get policy for.</param>
        /// <param name="defaultPolicy">Default policy (calculated from interface type).</param>
        /// <returns>Remote execution policy to be use when invoking this method.</returns>
        public static RemoteExecutionPolicy GetMethodPolicy(this MethodInfo me, RemoteExecutionPolicy defaultPolicy)
        {
            if (me == null)
                throw new ArgumentNullException(nameof(me));
            var timeoutPolicy = me.GetAttributeOrDefault<TimeoutPolicyAttribute>();
            var durablePolicy = me.GetAttributeOrDefault<DurablePolicyAttribute>();
            var ipPolicy = me.GetAttributeOrDefault<IpEndPointPolicyAttribute>();
            var returnPolicy = me.GetAttributeOrDefault<ForcedReturnPolicyAttribute>();

            var ans = me.GetAttributeOrDefault<RemoteExecutionPolicy>() ?? defaultPolicy;
            
            if (timeoutPolicy != null)
                ans.Timeout = timeoutPolicy.Timeout;
            if (durablePolicy != null)
            {
                ans.NoRetries = durablePolicy.NoRetries;
                ans.TimeoutIsStrict = durablePolicy.TimeoutIsStrict;
            }
            if (ipPolicy != null)
                ans.RequiresIpEndPoint = ipPolicy.Required;
            if (returnPolicy != null)
                ans.ForcedReturnPolicy = returnPolicy.ForcedReturnPolicy;
            if (me.ReturnType != typeof(void))
                ans.DefaultReturnPolicy = ReturnPolicy.TwoWay;

            return ans;
        }
    }
}