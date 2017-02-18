using System;
using System.Linq;
using RemoteExecution.Config;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the default remote execution policy for an interface.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="defaultNoResultsMethodExecution"></param>
        /// <returns></returns>
        public static RemoteExecutionPolicy GetInterfaceDefaultPolicy(this Type me,
            ReturnPolicy defaultNoResultsMethodExecution)
        {
            if (me == null)
                throw new ArgumentNullException(nameof(me));
            if (!me.IsInterface)
                throw new InvalidOperationException("Type must be an interface.");
            var timeoutPolicy = me.GetAttributeOrDefault<TimeoutPolicyAttribute>();
            var durablePolicy = me.GetAttributeOrDefault<DurablePolicyAttribute>();
            var ipPolicy = me.GetAttributeOrDefault<IpEndPointPolicyAttribute>();
            var returnPolicy = me.GetAttributeOrDefault<ForcedReturnPolicyAttribute>();

            var ans = me.GetAttributeOrDefault<RemoteExecutionPolicy>() ?? new RemoteExecutionPolicy
            {
                Timeout = DefaultConfig.Timeout,
                NoRetries = DefaultConfig.NoRetries,
                TimeoutIsStrict = DefaultConfig.TimeoutIsStrict,
                RequiresIpEndPoint = DefaultConfig.RequiresIpEndpoint,
                DefaultReturnPolicy = defaultNoResultsMethodExecution,
                ForcedReturnPolicy = null
            };

            ans.DefaultReturnPolicy = defaultNoResultsMethodExecution;
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

            return ans;
        }

        /// <summary>
        /// Gets a single attribute on a type. If the child doesn't have the attribute, the parent is checked.
        /// </summary>
        /// <typeparam name="T">Attribute to look for.</typeparam>
        /// <param name="me">Type to check.</param>
        /// <returns>Attribute if found, otherwise null.</returns>
        public static T GetAttributeOrDefault<T>(this Type me) where T : Attribute
        {
            return me.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
        }
    }
}