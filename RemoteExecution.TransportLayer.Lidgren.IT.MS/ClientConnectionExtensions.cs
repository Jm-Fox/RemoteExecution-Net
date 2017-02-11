using System.Net;
using System.Reflection;
using RemoteExecution.Channels;
using RemoteExecution.Connections;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    public static class ClientConnectionExtensions
    {
        public static IPEndPoint GetClientEndpoint(this ClientConnection me)
        {
            LidgrenDuplexChannel channel = (LidgrenDuplexChannel)
                me.GetType().GetField("Channel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(me);
            return new IPEndPoint(IPAddress.Loopback, channel.Connection.Peer.Port);
        }
    }
}