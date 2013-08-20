﻿using System;
using System.Collections.Generic;
using RemoteExecution.Core.Connections;

namespace RemoteExecution.Core.Endpoints
{
	/// <summary>
	/// Interface for server endpoint allowing to listen for incoming connections.
	/// </summary>
	public interface IServerEndpoint : IDisposable
	{
		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		IEnumerable<IRemoteConnection> ActiveConnections { get; }

		/// <summary>
		/// Returns true if endpoint is accepting incoming connections.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Starts accepting incoming connections.
		/// </summary>
		void Start();

		/// <summary>
		/// Fires when new connection is opened, it is fully configured and ready to use.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionOpened;

		/// <summary>
		/// Fires when connection has been closed and is no longer on active connections list.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionClosed;
	}
}
