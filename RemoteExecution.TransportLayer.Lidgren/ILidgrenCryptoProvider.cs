using System.Net;
using Lidgren.Network;

namespace RemoteExecution {
    /// <summary>
    /// Resolves encryption for a given source / destination. Assumes user-established encryption standards
    /// </summary>
    // Example:
    // Server has RSA private and public key;
    // 1. Client asks for public key
    // 2. Server replies with public key
    // 3. Client chooses an AES key and sends it encrypted with the RSA public key
    // 3.5 Client sets up Resolve to decrypt the server's messages with the chosen AES key
    // 4. Server receives request; decrypts it using the RSA private key, and sets up Resolve as well
    // 5. Server sends encrypted success message to the client
    public interface ILidgrenCryptoProvider {
        /// <summary>
        /// Used to look up the appropriate NetEncryption for traffic to/from a given IPEndPoint; null means unencrypted
        /// </summary>
        /// <param name="endPoint">IPEndPoint used to find the NetEncryption</param>
        /// <returns>The NetEncryption to use</returns>
        NetEncryption Resolve(IPEndPoint endPoint);
    }
}
