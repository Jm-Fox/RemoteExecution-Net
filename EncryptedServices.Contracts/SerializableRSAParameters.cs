using System;
using ProtoBuf;
using System.Security.Cryptography;

namespace EncryptedServices.Contracts
{
    /// <summary>
    /// SerializableRSAParameters exists because <see cref="RSAParameters"/> is not serializable.
    /// This class allows both the default MessageSerializer and the ProtobufMessageSerializer to work.
    /// </summary>
    [Serializable]
    [ProtoContract]
    // ReSharper disable once InconsistentNaming
    public class SerializableRSAParameters
    {
        [ProtoMember(1)]
        public byte[] Exponent;
        [ProtoMember(2)]
        public byte[] Modulus;
        [ProtoMember(3)]
        public byte[] P;
        [ProtoMember(4)]
        public byte[] Q;
        [ProtoMember(5)]
        // ReSharper disable once InconsistentNaming
        public byte[] DP;
        [ProtoMember(6)]
        // ReSharper disable once InconsistentNaming
        public byte[] DQ;
        [ProtoMember(7)]
        public byte[] InverseQ;
        [ProtoMember(8)]
        public byte[] D;
    }
}