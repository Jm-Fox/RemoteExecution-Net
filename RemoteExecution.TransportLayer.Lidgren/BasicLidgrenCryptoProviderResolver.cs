using System;
using System.Collections.Generic;
using System.Net;
using Lidgren.Network;

namespace RemoteExecution
{
    /// <summary>
    /// Basic Lidgren crypto providerResolver that wraps a Dictionary.
    /// </summary>
    public class BasicLidgrenCryptoProviderResolver : ILidgrenCryptoProviderResolver
    {
        private readonly Dictionary<IPEndPoint, NetEncryption> _endPointMap = new Dictionary<IPEndPoint, NetEncryption>();

        /// <summary>
        /// Registers a NetEncryption with a given IPEndPoint.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="provider"></param>
        /// <returns>True if successful, false if IPEndPoint already has encryption and registration was ignored.</returns>
        public bool Register(IPEndPoint endPoint, NetEncryption provider)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (_endPointMap.ContainsKey(endPoint))
                return false;
            _endPointMap[endPoint] = provider;
            return true;
        }

        /// <summary>
        /// Unregisters a IPEndPoint's NetEncryption.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns>True if successful, false if IPEndPoint wasn't registered.</returns>
        public bool Unregister(IPEndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            if (!_endPointMap.ContainsKey(endPoint))
                return false;
            _endPointMap.Remove(endPoint);
            return true;
        }

        /// <summary>
        /// Clears out all IPEndPoint mappings.
        /// </summary>
        public void Clear()
        {
            _endPointMap.Clear();
        }

        /// <summary>
        /// Resolves a NetEncryption from a given IPEndPoint.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns>NetEncryption if registered, otherwise null.</returns>
        public NetEncryption Resolve(IPEndPoint endPoint)
        {
            return _endPointMap.ContainsKey(endPoint) ? _endPointMap[endPoint] : null;
        }
    }
}
