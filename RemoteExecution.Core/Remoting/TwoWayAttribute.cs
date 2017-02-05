using System;

namespace RemoteExecution.Remoting
{
    /// <summary>
    /// Indicates that a void method should block (overrides NoResultMethodExecution.TwoWay)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TwoWayAttribute : Attribute
    {
    }
}