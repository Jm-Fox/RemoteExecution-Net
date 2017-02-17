using RemoteExecution.TransportLayer;
using System.Net;
using RemoteExecution.Remoting;

namespace SharedClientServices.Contracts
{
    public interface IBasicService
    {
        int Add(int x, int y);

        [OneWay]
        [RequiresIpEndPoint]
        void SayHello(string message, IPEndPoint endPoint = null);
    }
}