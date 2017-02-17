using System;

namespace RemoteExecution.Connections
{
    /// <summary>
    /// Indicates that a class is durable.
    /// </summary>
    public interface IDurableConnection
    {
        /// <summary>
        /// [Singular] event fired when the connection is closed. The listener can suggest a new host and port where the service might be located.
        /// If not used, reconnection attempts will hit the same host and port.
        /// </summary>
        Action<ClosedConnectionResponse> ConnectionPaused { get; set; }

        /// <summary>
        /// How many times the channel can try to reconnect before aborting.
        /// </summary>
        int RetryAttempts { get; set; }

        /// <summary>
        /// Event fired when connection is aborted.
        /// </summary>
        event Action ConnectionAborted;

        /// <summary>
        /// Event fired when connection is down.
        /// </summary>
        event Action ConnectionInterrupted;

        /// <summary>
        /// Event fired when connection is restored.
        /// </summary>
        event Action ConnectionRestored;
    }
}