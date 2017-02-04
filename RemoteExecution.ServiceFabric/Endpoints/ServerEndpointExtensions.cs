using System;
using RemoteExecution.Endpoints;

namespace RemoteExecution.ServiceFabric.Endpoints
{
    internal static class ServerEndpointExtensions
    {
        public static int GetPort(this ServerEndpoint me)
        {
            try
            {
                // NOTE that the _netServer is of type NetServer from Lidgren.Network.Core, and that Lidgren.Network.Core
                // cannot be referenced by a .net *.* project, only a netstandard project.
                // This *should* work, but reflection is mandatory for accessing Lidgren.Network.Core from a .net *.* project
                // todo: reconsider reflection
                object val = me.GetField("_listener")?.GetField("_netServer")?.GetProperty("Port");
                if (val == null)
                    return -1;
                return (int) val;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}