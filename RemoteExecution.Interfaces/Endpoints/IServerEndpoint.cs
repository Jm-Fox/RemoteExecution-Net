using System;
using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Interface for server endpoint allowing to listen for incoming connections.
	/// </summary>
	public interface IServerEndpoint : IDisposable
	{
		/// <summary>
		/// Fires when connection has been closed and is no longer on active connections list.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionClosed;

		/// <summary>
		/// Fires when new connection is opened, it is fully configured and ready to use.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionOpened;

		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		IEnumerable<IRemoteConnection> ActiveConnections { get; }

		/// <summary>
		/// Returns broadcast executor.
		/// </summary>
		IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; }

		/// <summary>
		/// Returns true if endpoint is accepting incoming connections.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Starts accepting incoming connections.
		/// </summary>
		void Start();

        /// <summary>
        /// Port (only defined if listening)
        /// </summary>
        int Port { get; }
	}
}
