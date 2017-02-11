using System.Net;
using RemoteExecution.InterfaceResolution;

namespace RemoteExecution.Protobuf.UT.MS
{
    internal interface ICalculator
    {
        int Add(int x, int y);

        [RequiresIpEndPoint]
        int Divide(int x, int y, IPEndPoint endPoint = null);
    }
}
