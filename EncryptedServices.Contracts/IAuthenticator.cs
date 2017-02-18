using System.Net;
using RemoteExecution.Executors;
using RemoteExecution.Remoting;

namespace EncryptedServices.Contracts
{
    public interface IAuthenticator
    {
        [ForcedReturnPolicy(ReturnPolicy.OneWay)]
        [IpEndPointPolicy(true)]
        void EncryptSession(byte[] encryptedSecret, IPEndPoint endPoint = null);

        SerializableRSAParameters GetRsaParameters();

        int Add(int x, int y);
    }
}
