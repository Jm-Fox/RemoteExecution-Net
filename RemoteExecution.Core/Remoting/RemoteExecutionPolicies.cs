using System;
using System.Collections.Generic;
using System.Reflection;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Collection of RemoteExecutionPolicyAttributes (unique per interface).
    /// </summary>
    public class RemoteExecutionPolicies : Dictionary<MethodInfo, RemoteExecutionPolicy>
    {
        /// <summary>
        /// Populates dictionary with policies for a given interface.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="defaultNoResultsMethodExecution"></param>
        public RemoteExecutionPolicies(Type interfaceType, ReturnPolicy defaultNoResultsMethodExecution)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            RemoteExecutionPolicy defaultPolicy =
                interfaceType.GetInterfaceDefaultPolicy(defaultNoResultsMethodExecution);
            AddMethodsForInterface(interfaceType, defaultPolicy);
        }

        private void AddMethodsForInterface(Type interfaceType, RemoteExecutionPolicy defaultPolicy)
        {
            Queue<Type> interfaceTypes = new Queue<Type>();
            interfaceTypes.Enqueue(interfaceType);
            while (interfaceTypes.Count > 0)
            {
                Type dequeue = interfaceTypes.Dequeue();
                foreach (MethodInfo method in dequeue.GetMethods())
                    if (!ContainsKey(method))
                        Add(method, method.GetMethodPolicy(defaultPolicy));
                foreach (Type inherited in dequeue.GetInterfaces())
                    interfaceTypes.Enqueue(inherited);
            }
        }
    }
}