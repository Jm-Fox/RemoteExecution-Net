using System;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Channels;

namespace RemoteExecution
{
    /// <summary>
    /// Synchronization extensions for networking
    /// </summary>
	public static class SynchronizationExtensions
	{
		private static readonly TimeSpan _synchronizationTimeSpan = TimeSpan.FromMilliseconds(25);

        /// <summary>
        /// Wait for a netpeer to close
        /// </summary>
        /// <param name="netPeer"></param>
		public static void WaitForClose(this NetPeer netPeer)
		{
			while (netPeer.Status != NetPeerStatus.NotRunning)
				Thread.Sleep(_synchronizationTimeSpan);
		}

        /// <summary>
        /// Wait for a connection to open
        /// </summary>
        /// <param name="connection"></param>
		public static void WaitForConnectionToOpen(this NetConnection connection)
		{
			while (connection.Status != NetConnectionStatus.Connected)
			{
				if (connection.Status == NetConnectionStatus.Disconnected)
					throw new ConnectionOpenException("Connection closed.");

				Thread.Sleep(_synchronizationTimeSpan);
			}
		}
	}
}