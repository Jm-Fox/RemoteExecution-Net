using System;
using System.Net;
using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Serializers;

namespace RemoteExecution
{
    /// <summary>
    /// Extends the Lidgren transport layer provider to have (1) durable clients which will attempt reconnection and
    /// (2) aggregate clients: only one port will be used for all connections
    /// </summary>
    public class SharedLidgrenProvider : LidgrenProvider
    {
        /// <summary>
        /// Default constructor; will use an <see cref="UnencryptedCryptoProviderResolver"/> for encryption
        /// </summary>
	    public SharedLidgrenProvider() { }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="cryptoProviderResolver">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s</param>
        public SharedLidgrenProvider(ILidgrenCryptoProviderResolver cryptoProviderResolver)
            : base (cryptoProviderResolver)
        {
        }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="messageSerializer">Message serializer to use.</param>
        public SharedLidgrenProvider(IMessageSerializer messageSerializer)
            : base (messageSerializer)
        {
        }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="cryptoProviderResolver">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s.</param>
        /// <param name="messageSerializer">Message serializer to use.</param>
        public SharedLidgrenProvider(ILidgrenCryptoProviderResolver cryptoProviderResolver, IMessageSerializer messageSerializer)
            : base(cryptoProviderResolver, messageSerializer)
        {
        }


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
            return new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer, CryptoProviderResolver);
        }

        /// <summary>
        /// Creates client channel for given uri.
        /// </summary>
        /// <param name="clientId">Identifier used to share NetClients between channels</param>
        /// <param name="uri">Uri used to configure client channel.</param>
        /// <returns>Client channel.</returns>
        /// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
        public override IClientChannel CreateClientChannelFor(string clientId, Uri uri)
        {
            VerifyScheme(uri);
            return new SharedLidgrenClientChannel(clientId,
                new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer,
                    CryptoProviderResolver));
        }
    }
}
