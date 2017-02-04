namespace RemoteExecution
{
    /// <summary>
    /// Plain-old-data for responding to a connection that closed unexpectedly.
    /// </summary>
    public class ClosedConnectionResponse
    {
        /// <summary>
        /// Connection should fail.
        /// </summary>
        public bool Fail;
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
