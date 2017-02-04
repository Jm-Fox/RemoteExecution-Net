using System;
using System.Linq;

namespace RemoteExecution
{
    /// <summary>
    /// Indicates that an contract method requires an IpEndPoint (client and server interfaces differ).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequiresIpEndPointAttribute : Attribute
    {
        /// <summary>
        /// Determines whether or not an interface type's method requires an IpEndPoint
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        // This method allows the following two methods to match:
        // Client.ICalculator: { int Add_(int x, int y); }
        // Server.ICalculator: { int Add_(int x, int y, IPEndPoint clientAddress);}
        public static bool RequiresIpEndPoint(Type type, string method)
        {
            return type.GetMethods()
                .Single(m => m.Name == method)
                .GetCustomAttributes(typeof(RequiresIpEndPointAttribute), true)
                .Any();
        }
    }
}
