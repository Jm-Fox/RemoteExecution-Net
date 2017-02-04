using System;

namespace RemoteExecution
{
    /// <summary>
    /// Class for resolving an interface type from the name of the interface.
    /// No implementation is provided- you need to write your own and register it with
    /// InterfaceResolver.Singleton = new MyInterfaceResolver();
    /// </summary>
    public abstract class InterfaceResolver
    {
        /// <summary>
        /// Singleton for access. 
        /// </summary>
        public static InterfaceResolver Singleton { get; set; }

        /// <summary>
        /// Resolves an interface type from the name of the interface
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public abstract Type GetInterface(string interfaceName);
    }
}
