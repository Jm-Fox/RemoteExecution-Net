using System.Net;
using Lidgren.Network;

namespace RemoteExecution {
    /// <summary>
    /// Doesn't provide any encryption
    /// </summary>
    public class UnencryptedCryptoProvider : ILidgrenCryptoProvider {
        /// <summary>
        /// Always returns null
        /// </summary>
        /// <param name="endPoint">IPEndPoint used to find the NetEncryption (ignored)</param>
        /// <returns>The NetEncryption to use (always null)</returns>
        public NetEncryption Resolve(IPEndPoint endPoint)
        {
            return null;
        }
    }
}
