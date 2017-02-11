using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Lidgren.Network;

namespace RemoteExecution.Channels
{
    /// <summary>
    /// Allows multiple channels to share one NetPeer (and most importantly, one port)
    /// </summary>
    public class SharedLidgrenClientChannel : LidgrenClientChannelWrapper
    {
        #region New fields

        private readonly string clientId;

        private bool started;

        private static readonly ConcurrentDictionary<string, SharedLidgrenClientChannel> SharedChannels =
            new ConcurrentDictionary<string, SharedLidgrenClientChannel>();

        #endregion

        /// <summary>
        /// Creates client channel instance.
        /// </summary>
        public SharedLidgrenClientChannel(string clientId, LidgrenClientChannel inner)
            : base(inner)
        {
            this.clientId = clientId;
            if (SharedChannels.ContainsKey(clientId))
            {
                LidgrenClientChannel shared = SharedChannels[clientId].Inner;
                inner.Client = shared.Client;
            }
            else
            {
                // Force Client to be a NetPeer, not a NetClient
                Inner.Client = new NetPeer(Inner.Client.Configuration);
                // Ensure incoming connections are disabled
                Inner.Client.Configuration.AcceptIncomingConnections = false;
                SharedChannels[clientId] = this;
            }
        }

        #region Modified methods

        /// <summary>
        /// Closes channel. 
        /// It should not throw if channel is already closed.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Close()
        {
            Inner.Close();
            SharedLidgrenClientChannel x;
            SharedChannels.TryRemove(clientId, out x);
        }

        /// <summary>
        /// Opens channel for sending and receiving messages.
        /// If channel has been closed, this method reopens it.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Open()
        {
            if (IsOpen)
                return;
            if (SharedChannels[clientId].started)
            {
                Inner.Connection = Client.Connect(Host, Port);
                Inner.Connection.WaitForConnectionToOpen();
            }
            else
            {
                started = true;
                Inner.Open();
            }
        }

        #endregion
    }
}