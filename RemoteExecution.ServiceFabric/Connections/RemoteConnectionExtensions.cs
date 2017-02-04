using System;
using System.Reflection;
using RemoteExecution.Channels;
using RemoteExecution.Connections;

namespace RemoteExecution.ServiceFabric.Connections
{
    internal static class RemoteConnectionExtensions
    {
        private static readonly FieldInfo ChannelFieldInfo = typeof(RemoteConnection).GetField("Channel",
            BindingFlags.Instance | BindingFlags.NonPublic);

        public static void SetConnectionClosed(this RemoteConnection me,
            Func<ClosedConnectionResponse> @event)
        {
            // todo: expose internal? reflection shouldn't be used here...
            DurableLidgrenClientChannel channel = (DurableLidgrenClientChannel) ChannelFieldInfo.GetValue(me);
            channel.ConnectionClosed = @event;
        }
    }
}
