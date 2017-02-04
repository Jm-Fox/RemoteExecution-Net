using Lidgren.Network;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    internal delegate void CryptoEvent(byte[] before, byte[] after);

    // ReSharper disable once InconsistentNaming
    internal class ObservableAESEncryption : NetAESEncryption
    {
        public CryptoEvent OnEncrypt;
        public CryptoEvent OnDecrypt;

        public ObservableAESEncryption(string appId, string secret)
            : base(new NetPeer(new NetPeerConfiguration(appId)), secret)
        {
        }

        public override bool Decrypt(NetIncomingMessage msg)
        {
            byte[] before = new byte[msg.LengthBytes];
            msg.PeekBytes(msg.LengthBytes).CopyTo(before, 0);
            bool ans = base.Decrypt(msg);
            byte[] after = new byte[msg.LengthBytes];
            msg.PeekBytes(msg.LengthBytes).CopyTo(after, 0);
            OnDecrypt?.Invoke(before, after);
            return ans;
        }

        public override bool Encrypt(NetOutgoingMessage msg)
        {
            byte[] before = new byte[msg.LengthBytes];
            msg.PeekBytes(msg.LengthBytes).CopyTo(before, 0);
            bool ans = base.Encrypt(msg);
            byte[] after = new byte[msg.LengthBytes];
            msg.PeekBytes(msg.LengthBytes).CopyTo(after, 0);
            OnEncrypt?.Invoke(before, after);
            return ans;
        }
    }
}