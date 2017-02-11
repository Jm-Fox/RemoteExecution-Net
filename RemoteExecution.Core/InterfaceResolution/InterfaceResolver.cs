using System;
using System.Linq;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.InterfaceResolution
{
    /// <summary>
    /// Class for resolving an interface type from the name of the interface.
    /// </summary>
    public abstract class InterfaceResolver
    {
        /// <summary>
        /// Singleton for access. 
        /// </summary>
        public static InterfaceResolver Singleton { get; set; } = new BasicInterfaceResolver();

        /// <summary>
        /// Resolves an interface type from the name of the interface.
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public abstract Type GetInterface(string interfaceName);

        /// <summary>
        /// Registers a name mapping to the type of the interface.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns>True if successfully added, false if ignored because the interface name already is registered.</returns>
        public abstract bool RegisterInterface(Type interfaceType);

        /// <summary>
        /// Determines whether or not an interface type's method requires an IpEndPoint
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool RequiresIpEndPoint(Type type, string method)
        {
            return type.GetMethods()
                .Single(m => m.Name == method)
                .CustomAttributes
                .Any(a => a.AttributeType.AssemblyQualifiedName == typeof(RequiresIpEndPointAttribute).AssemblyQualifiedName);
            // For whatever reason (a => a is RequiresIpEndPointAttribute), and its variants, are always false, despite the types appearing identical.
        }

        /// <summary>
        /// Determines whether or not the channel should inject the IPAddress into the method arguments.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool SenderEndPointIsExpectedByInterface(IRequestMessage request)
        {
            return RequiresIpEndPoint(Singleton.GetInterface(request.MessageType), request.MethodName);
        }
    }
}
