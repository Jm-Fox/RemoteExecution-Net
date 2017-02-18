using System;
using System.Net;
using RemoteExecution.Remoting;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    public interface IEndPointProvisioner
    {
        [IpEndPointPolicy(true)]
        void DoStuff(IPEndPoint endPoint = null);
    }

    public class EndPointProvisioner : IEndPointProvisioner
    {
        public Action<IPEndPoint> OnStuff;

        public void DoStuff(IPEndPoint endPoint)
        {
            OnStuff?.Invoke(endPoint);
        }
    }
}
