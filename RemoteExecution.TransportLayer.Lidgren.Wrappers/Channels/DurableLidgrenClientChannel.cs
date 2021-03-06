﻿using System;
using System.Collections.Concurrent;
using Lidgren.Network;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
    /// <summary>
    /// Provides a durably implementation of the LidgrenClientChannel. If the remote service has failed,
    /// it's possible that it will be restored within a few seconds. While this logic could easily be
    /// implemented elsewhere, it makes the most sense to have it here.
    /// </summary>
    public class DurableLidgrenClientChannel : LidgrenClientChannelWrapper, IDurableConnection
    {
        #region Durable fields and properties

        /// <summary>
        /// Whether or not this channel is actually closed (since IsOpen can be false temporariliy) 
        /// </summary>
        private bool actuallyClosed = false;

        /// <summary>
        /// How many reconnection failures have occurred since the last successful connection
        /// </summary>
        private int failures = 0;

        /// <summary>
        /// Whether or not this channel is closing down gracefully (initiated by program as opposed to an unexpected connection failure)
        /// </summary>
        private bool gracefulClosing = false;

        private bool attemptingReconnection = false;

        /// <summary>
        /// [Singular] event fired when the connection is closed. The listener can suggest a new host and port where the service might be located.
        /// If not used, reconnection attempts will hit the same host and port.
        /// </summary>
        public Action<PausedConnectionResponse> ConnectionPaused { get; set; }

        /// <summary>
        /// Event fired when connection is restored.
        /// </summary>
        public event Action ConnectionRestored;

        /// <summary>
        /// Event fired when connection is aborted.
        /// </summary>
        public event Action ConnectionAborted;

        /// <summary>
        /// Event fired when connection is down.
        /// </summary>
        public event Action ConnectionInterrupted;

        private bool hasPendingOutgoing = false;

        private readonly object dumpPendingOutgoingLock = new object();

        /// <summary>
        /// Bytes that are awaiting sending due to temporary shutdown of the connection
        /// </summary>
        protected ConcurrentBag<byte[]> PendingOutgoing = new ConcurrentBag<byte[]>();

        /// <summary>
        /// How many times the channel can try to reconnect before aborting.
        /// </summary>
        public int RetryAttempts { get; set; } = 3;

        #endregion

        /// <summary>
        /// Wraps a LidgrenClientChannel.
        /// </summary>
        /// <param name="inner"></param>
        public DurableLidgrenClientChannel(LidgrenClientChannel inner) : base(inner)
        {
        }

        #region Modified methods

        /// <summary>
        /// Closes channel. 
        /// It should not throw if channel is already closed.
        /// </summary>
        public override void Close()
        {
            gracefulClosing = true;
            base.Close();
        }

        /// <summary>
        /// Handles lidgren connection close event (fires Closed event).
        /// </summary>
        public override void OnConnectionClose()
        {
            if (!gracefulClosing && !attemptingReconnection)
            {
                ConnectionInterrupted?.Invoke();
                attemptingReconnection = true;
                PausedConnectionResponse response  = new PausedConnectionResponse
                {
                    ReconnectHost = Host,
                    ReconnectPort = Port
                };
                while (!gracefulClosing && failures < RetryAttempts)
                {
                    response.FailedAttempts = failures;
                    ConnectionPaused?.Invoke(response);
                    if (response.Abort)
                        break;
                    NetConnection newConnection = Client.Connect(response.ReconnectHost, response.ReconnectPort);
                    try
                    {
                        newConnection.WaitForConnectionToOpen();
                        RestoreConnection(newConnection, response);
                        ConnectionRestored?.Invoke();
                        return;
                    }
                    catch (ConnectionOpenException)
                    {
                        failures++;
                    }
                }
                ConnectionAborted?.Invoke();
                AbortConnection();
            }
        }

        /// <summary>
        /// Sends given message through this channel.
        /// </summary>
        /// <param name="message">Message to send.</param>
		/// <returns>True if the connection is open, false if the connection is paused.</returns>
        public override bool Send(IMessage message)
        {
            return SendData(Inner.Serializer.Serialize(message));
        }

        /// <summary>
        /// Sends data through channel.
        /// </summary>
        /// <param name="data">Data to send.</param>
		/// <returns>True if the connection is open, false if the connection is paused.</returns>
        public override bool SendData(byte[] data)
        {
            if (actuallyClosed)
            {
                throw new NotConnectedException("Network connection is not opened.");
            }
            if (!IsOpen)
            {
                PendingOutgoing.Add(data);
                hasPendingOutgoing = true;
                return false;
            }
            // A(n extremely rare) race condition can leave messages in the Queue, not to be resent until the next failure.
            // This ensures no message is left behind.
            if (hasPendingOutgoing)
                DumpPendingOutgoing();
            Inner.SendData(data);
            return true;
        }

        #endregion

        #region New methods

        private void AbortConnection()
        {
            actuallyClosed = true;
            attemptingReconnection = false;
            base.OnConnectionClose();
            Dispose();
        }
        
        private void RestoreConnection(NetConnection connection, PausedConnectionResponse response)
        {
            Connection = connection;
            attemptingReconnection = false;
            Host = response.ReconnectHost;
            Port = response.ReconnectPort;
            failures = 0;
            DumpPendingOutgoing();
        }
        
        private void DumpPendingOutgoing()
        {
            DefaultConfig.TaskScheduler.Execute(() =>
            {
                lock (dumpPendingOutgoingLock)
                {
                    while (!PendingOutgoing.IsEmpty)
                    {
                        PendingOutgoing.TryTake(out byte[] data);
                        Inner.SendData(data);
                    }
                    hasPendingOutgoing = false;
                }
            });
        }

        #endregion
    }
}