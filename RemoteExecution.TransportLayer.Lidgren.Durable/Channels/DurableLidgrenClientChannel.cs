using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Lidgren.Network;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
    /// <summary>
    /// Provides a durably implementation of the LidgrenClientChannel. If the remote service has failed,
    /// it's possible that it will be restored within a few seconds. While this logic could easily be
    /// implemented elsewhere, it makes the most sense to have it here.
    /// </summary>
    public class DurableLidgrenClientChannel : AggregateLidgrenClientChannel
    {
        /// <summary>
        /// How many reconnection failures have occurred since the last successful connection
        /// </summary>
        private int failures = 0;

        /// <summary>
        /// Whether or not this channel is closing down gracefully (initiated by program as opposed to an unexpected connection failure)
        /// </summary>
        private bool gracefulClosing = false;

        /// <summary>
        /// How many times the channel can try to reconnect before breaking
        /// </summary>
        [DefaultValue(3)]
        public int RetryAttempts { get; set; }

        /// <summary>
        /// [Singular] event fired when the connection is closed. The listener can suggest a new host and port where the service might be located.
        /// If not used, reconnection attempts will hit the same host and port.
        /// </summary>
        public Func<ClosedConnectionResponse> ConnectionClosed;

        /// <summary>
        /// Creates durable client channel instance.
        /// </summary>
        /// <param name="applicationId">Application id that has to match to one used by <see cref="LidgrenServerConnectionListener"/>.</param>
        /// <param name="host">Host to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        /// <param name="serializer">Message serializer.</param>
        public DurableLidgrenClientChannel(string applicationId, string host, ushort port, IMessageSerializer serializer)
            : base(applicationId, host, port, serializer)
        {
        }

        /// <summary>
        /// Creates durable client channel instance.
        /// </summary>
        /// <param name="retryAttempts">How many times the channel can try to reconnect before breaking</param>
        /// <param name="applicationId">Application id that has to match to one used by <see cref="LidgrenServerConnectionListener"/>.</param>
        /// <param name="host">Host to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        /// <param name="serializer">Message serializer.</param>
        public DurableLidgrenClientChannel(int retryAttempts, string applicationId, string host, ushort port,
            IMessageSerializer serializer)
            : base(applicationId, host, port, serializer)
        {
            RetryAttempts = retryAttempts;
        }

        /// <summary>
        /// Bytes that are awaiting sending due to temporary shutdown of the connection
        /// </summary>
        protected ConcurrentBag<byte[]> PendingOutgoing = new ConcurrentBag<byte[]>();

        /// <summary>
        /// Sends data through channel.
        /// </summary>
        /// <param name="data">Data to send.</param>
        protected override void SendData(byte[] data)
        {
            if (!IsOpen)
            {
                PendingOutgoing.Add(data);
            }
            else
            {
                // A(n extremely rare) race condition can leave messages in the Queue, not to be resent until the next failure.
                // This ensures no message is left behind.
                DumpPendingOutgoing();
                Connection.Peer.SendMessage(CreateOutgoingMessage(data), Connection, NetDeliveryMethod.ReliableUnordered,
                    0);
            }
        }

        /// <summary>
        /// Handles lidgren connection close event (fires Closed event).
        /// </summary>
        public override void OnConnectionClose()
        {
            if (!gracefulClosing)
            {
                ClosedConnectionResponse response = null;
                if (ConnectionClosed != null)
                {
                    response = ConnectionClosed();
                }
                if (response == null)
                {
                    response = new ClosedConnectionResponse
                    {
                        Fail = false,
                        ReconnectHost = Host,
                        ReconnectPort = Port
                    };
                }
                if (!response.Fail)
                {
                    while (!gracefulClosing && failures < RetryAttempts)
                    {
                        NetConnection newConnection = Client.Connect(response.ReconnectHost, response.ReconnectPort);
                        try
                        {
                            newConnection.WaitForConnectionToOpen();
                            Connection = newConnection;
                            RestoreConnection(response);
                            return;
                        }
                        catch (ConnectionOpenException)
                        {
                            failures++;
                        }
                    }
                }
                base.OnConnectionClose();
                Dispose();
                throw new OperationAbortedException("Connection was lost and could not be restored.");
            }
            base.OnConnectionClose();
        }

        private void RestoreConnection(ClosedConnectionResponse response)
        {
            Host = response.ReconnectHost;
            Port = response.ReconnectPort;
            failures = 0;
            DumpPendingOutgoing();
        }

        private void DumpPendingOutgoing()
        {
            while (!PendingOutgoing.IsEmpty)
            {
                byte[] data;
                PendingOutgoing.TryTake(out data);
                SendData(data);
            }
        }

        /// <summary>
        /// Closes channel. 
        /// It should not throw if channel is already closed.
        /// </summary>
        protected override void Close()
        {
            gracefulClosing = true;
            base.Close();
        }
    }
}