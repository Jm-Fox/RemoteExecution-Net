using System.Net;
using RemoteExecution.Remoting;

namespace RemoteExecution.Protobuf.UT.MS
{
    internal interface ICalculator
    {
        int Add(int x, int y);

        [IpEndPointPolicy(true)]
        int Divide(int x, int y, IPEndPoint endPoint = null);
    }
}
