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