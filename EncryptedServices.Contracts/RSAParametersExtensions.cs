using System.Security.Cryptography;

namespace EncryptedServices.Contracts
{
    // ReSharper disable once InconsistentNaming
    public static class RSAParametersExtensions
    {
        public static SerializableRSAParameters ToSerializable(this RSAParameters me)
        {
            return new SerializableRSAParameters
            {
                Exponent = me.Exponent,
                Modulus = me.Modulus,
                P = me.P,
                Q = me.Q,
                DP = me.DP,
                DQ = me.DQ,
                InverseQ = me.InverseQ,
                D = me.D
            };
        }

        public static RSAParameters ToParams(this SerializableRSAParameters me)
        {
            return new RSAParameters
            {
                Exponent = me.Exponent,
                Modulus = me.Modulus,
                P = me.P,
                Q = me.Q,
                DP = me.DP,
                DQ = me.DQ,
                InverseQ = me.InverseQ,
                D = me.D
            };
        }
    }
}