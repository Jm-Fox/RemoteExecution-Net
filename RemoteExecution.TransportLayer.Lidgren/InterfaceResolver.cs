using System;

namespace RemoteExecution
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
    }
}
