using System;
using Lidgren.Network;

namespace EncryptedServices.Client
{
    // ReSharper disable once InconsistentNaming
    public class DebugNetAESEncryption : NetAESEncryption
    {
        public DebugNetAESEncryption(NetPeer peer) : base(peer)
        {
        }

        public DebugNetAESEncryption(NetPeer peer, string key) : base(peer, key)
        {
        }

        public DebugNetAESEncryption(NetPeer peer, byte[] data, int offset, int count) : base(peer, data, offset, count)
        {
        }

        public override bool Encrypt(NetOutgoingMessage msg)
        {
            Console.WriteLine("Bytes to encrypt: " + Convert.ToBase64String(msg.Data));
            bool ans = base.Encrypt(msg);
            Console.WriteLine("Encrypted bytes: " + Convert.ToBase64String(msg.Data));
            return ans;
        }

        public override bool Decrypt(NetIncomingMessage msg)
        {
            Console.WriteLine("Bytes to decrypt: " + Convert.ToBase64String(msg.Data));
            bool ans = base.Decrypt(msg);
            Console.WriteLine("Decrypted bytes: " + Convert.ToBase64String(msg.Data));
            return ans;
        }
    }
}
