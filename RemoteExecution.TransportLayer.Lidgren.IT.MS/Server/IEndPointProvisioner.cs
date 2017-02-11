using System;
using System.Net;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS.Server
{
    public interface IEndPointProvisioner
    {
        [RequiresIpEndPoint]
        void DoStuff(IPEndPoint endPoint);
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
