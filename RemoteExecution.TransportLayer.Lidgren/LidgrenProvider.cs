using System;
using System.Net;
using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	/// <summary>
	/// Lidgren transport layer providerResolver allowing to create client channel or server connection listener objects for given uri using Lidgren framework.
	/// </summary>
	public class LidgrenProvider : ITransportLayerProvider
	{
        /// <summary>
        /// Object used to serialize objects into byte arrays
        /// </summary>
	    protected readonly IMessageSerializer _serializer;

        /// <summary>
        /// Designated encryption providerResolver
        /// </summary>
	    protected readonly ILidgrenCryptoProviderResolver CryptoProviderResolver;

        /// <summary>
        /// Default constructor; will use an <see cref="UnencryptedCryptoProviderResolver"/> for encryption
        /// </summary>
	    public LidgrenProvider()
            : this(new UnencryptedCryptoProviderResolver(), DefaultConfig.MessageSerializer)
	    {
        }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="cryptoProviderResolver">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s</param>
        public LidgrenProvider(ILidgrenCryptoProviderResolver cryptoProviderResolver)
            : this (cryptoProviderResolver, DefaultConfig.MessageSerializer)
        {
        }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="messageSerializer">Message serializer to use.</param>
        public LidgrenProvider(IMessageSerializer messageSerializer)
            : this (new UnencryptedCryptoProviderResolver(), messageSerializer)
        {
        }

        /// <summary>
        /// Normal constructor; will use specified <see cref="ILidgrenCryptoProviderResolver"/>.
        /// </summary>
        /// <param name="cryptoProviderResolver">Provider to map <see cref="IPEndPoint"/>s to their corresponding <see cref="NetEncryption"/>s.</param>
        /// <param name="messageSerializer">Message serializer to use.</param>
        public LidgrenProvider(ILidgrenCryptoProviderResolver cryptoProviderResolver, IMessageSerializer messageSerializer)
        {
            CryptoProviderResolver = cryptoProviderResolver;
            _serializer = messageSerializer;
        }

        #region ITransportLayerProvider Members

        /// <summary>
        /// Creates client channel for given uri.
        /// This implementation supports scheme in following format: net://[host]:[port]/[applicationId]
        /// </summary>
        /// <param name="uri">Uri used to configure client channel.</param>
        /// <returns>Client channel.</returns>
        /// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
        public virtual IClientChannel CreateClientChannelFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer, CryptoProviderResolver);
		}

		/// <summary>
		/// Creates server connection listener for given uri.
		/// This implementation supports scheme in following format: net://[ip_to_listen_on]:[port]/[applicationId]
		/// [ip_to_listen_on] could be 0.0.0.0, which means that connection listener would be listening on all network interfaces.
		/// </summary>
		/// <param name="uri">Uri used to configure server connection listener.</param>
		/// <returns>Server connection listener</returns>
		/// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
		public IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenServerConnectionListener(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer, CryptoProviderResolver);
		}

		/// <summary>
		/// Returns supported scheme.
		/// This implementation supports net:// scheme.
		/// </summary>
		public string Scheme { get { return "net"; } }

		#endregion

        /// <summary>
        /// Gets the application ID from a Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
	    protected string GetApplicationId(Uri uri)
		{
			var applicationId = uri.LocalPath.Trim('/');

			if (string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentException("No application id provided.");

			if (applicationId.Contains("/"))
				throw new ArgumentException("Application id cannot contain '/' character.");
			return applicationId;
		}

        /// <summary>
        /// Gets the port from a Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
	    protected ushort GetPort(Uri uri)
		{
			if (uri.Port <= 0)
				throw new ArgumentException("No port provided.");
			return (ushort)uri.Port;
		}

        /// <summary>
        /// Verifies that the current scheme matches a Uri's scheme
        /// </summary>
        /// <param name="uri"></param>
	    protected void VerifyScheme(Uri uri)
		{
			if (uri.Scheme != Scheme)
				throw new ArgumentException("Invalid scheme.");
		}

        /// <summary>
        /// Creates client channel for given uri.
        /// </summary>
        /// <param name="clientId">Unused by base LidgrenProvider implementation</param>
        /// <param name="uri">Uri used to configure client channel.</param>
        /// <returns>Client channel.</returns>
        /// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
        public virtual IClientChannel CreateClientChannelFor(string clientId, Uri uri)
        {
            throw new NotSupportedException();
        }
	}
}