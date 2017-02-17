using System.Net;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.Remoting;
using RemoteExecution.TransportLayer;

namespace EncryptedServices.Contracts
{
    public interface IAuthenticator
    {
        [OneWay]
        [RequiresIpEndPoint]
        void EncryptSession(byte[] encryptedSecret, IPEndPoint endPoint = null);

        SerializableRSAParameters GetRsaParameters();

        int Add(int x, int y);
    }
}
