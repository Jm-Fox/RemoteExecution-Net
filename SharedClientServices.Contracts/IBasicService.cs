using System.Net;
using RemoteExecution.Executors;
using RemoteExecution.Remoting;

namespace SharedClientServices.Contracts
{
    public interface IBasicService
    {
        int Add(int x, int y);

        [ForcedReturnPolicy(ReturnPolicy.OneWay)]
        [IpEndPointPolicy(true)]
        void SayHello(string message, IPEndPoint endPoint = null);
    }
}