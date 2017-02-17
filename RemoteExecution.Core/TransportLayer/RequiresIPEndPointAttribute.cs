using System;

namespace RemoteExecution.TransportLayer
{
    /// <summary>
    /// Indicates that an contract method requires an IpEndPoint (client and server interfaces differ).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequiresIpEndPointAttribute : Attribute
    {
    }
}
