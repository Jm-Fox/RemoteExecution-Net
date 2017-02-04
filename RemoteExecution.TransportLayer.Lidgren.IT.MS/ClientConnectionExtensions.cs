using System.Net;
using System.Reflection;
using RemoteExecution.Channels;
using RemoteExecution.Connections;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    internal static class ClientConnectionExtensions
    {
        public static IPEndPoint GetClientEndpoint(this ClientConnection me)
        {
            LidgrenDuplexChannel channel = (LidgrenDuplexChannel)
                me.GetType().GetField("Channel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(me);
            object netConnection = channel.GetType().GetProperty("Connection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(channel);
            object netPeer = netConnection.GetType().GetProperty("Peer").GetValue(netConnection);
            int port = (int)netPeer.GetType().GetProperty("Port").GetValue(netPeer);
            return new IPEndPoint(IPAddress.Loopback, port);
        }
    }
}