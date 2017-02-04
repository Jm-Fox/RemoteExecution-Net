using System;
using System.Collections.Generic;

namespace RemoteExecution
{
    /// <summary>
    /// Basic implementation of InterfaceResolver.
    /// </summary>
    public class BasicInterfaceResolver : InterfaceResolver
    {
        private Dictionary<string, Type> _nameTypeMap = new Dictionary<string, Type>();

        /// <summary>
        /// Looks up the interface name. If not found, a <see cref="KeyNotFoundException" /> exception is thrown.
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public override Type GetInterface(string interfaceName)
        {
            return _nameTypeMap[interfaceName];
        }

        /// <summary>
        /// Registers a name mapping to the type of the interface.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns>True if successfully added, false if ignored because the interface name already is registered.</returns>
        public override bool RegisterInterface(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException("RegisterInterface only accepts types which are interfaces.");
            if (_nameTypeMap.ContainsKey(interfaceType.Name))
                return false;
            _nameTypeMap[interfaceType.Name] = interfaceType;
            return true;
        }
    }
}
