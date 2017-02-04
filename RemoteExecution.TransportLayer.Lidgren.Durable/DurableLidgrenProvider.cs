using System;
using RemoteExecution.Channels;

namespace RemoteExecution
{
    /// <summary>
    /// Extends the Lidgren transport layer provider to have (1) durable clients which will attempt reconnection and
    /// (2) aggregate clients: only one port will be used for all connections
    /// </summary>
    public class DurableLidgrenProvider : LidgrenProvider
    {
        /// <summary>
        /// Creates durable client channel for given uri.
        /// This implementation supports scheme in following format: net://[host]:[port]/[applicationId]
        /// </summary>
        /// <param name="uri">Uri used to configure client channel.</param>
        /// <returns>Client channel.</returns>
        /// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
        public override IClientChannel CreateClientChannelFor(Uri uri)
        {
            VerifyScheme(uri);
            return new DurableLidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
        }
    }
}
