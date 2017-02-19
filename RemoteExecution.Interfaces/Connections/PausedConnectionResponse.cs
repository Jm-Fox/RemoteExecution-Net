namespace RemoteExecution.Connections
{
    /// <summary>
    /// Plain-old-data for responding to a connection that closed unexpectedly.
    /// </summary>
    public class PausedConnectionResponse
    {
        /// <summary>
        /// How many failed attempts have been made to restore the connection.
        /// </summary>
        public int FailedAttempts = 0;
        /// <summary>
        /// Connection should fail.
        /// </summary>
        public bool Abort = false;
        /// <summary>
        /// Connection should retry with specified host (may or may not be different).
        /// </summary>
        public string ReconnectHost;
        /// <summary>
        /// Connection should retry with specified port (may or may not be different).
        /// </summary>
        public ushort ReconnectPort;
    }
}
