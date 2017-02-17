using System;
using System.Net;
using System.Security.Cryptography;
using EncryptedServices.Contracts;
using Lidgren.Network;

namespace EncryptedServices.Server
{
    public class Authenticator : IAuthenticator
    {
        private readonly RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private readonly NetPeer emptyPeer = new NetPeer(new NetPeerConfiguration("*"));

        public readonly ISessionEncryptedCallback Callback;

        public Authenticator(ISessionEncryptedCallback callback)
        {
            Callback = callback;
        }

        public void EncryptSession(byte[] encryptedSecret, IPEndPoint endPoint)
        {
            Callback.ConfirmEncryption(true);
            byte[] decryptedSecret = csp.Decrypt(encryptedSecret, false);
            Program.ProviderResolver.Register(endPoint, new NetAESEncryption(emptyPeer, Convert.ToBase64String(decryptedSecret)));
        }

        public SerializableRSAParameters GetRsaParameters()
        {
            return csp.ExportParameters(false).ToSerializable();
        }

        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}