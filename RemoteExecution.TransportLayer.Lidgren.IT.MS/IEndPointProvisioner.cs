using System;
using System.Net;
using RemoteExecution.InterfaceResolution;

namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
    public interface IEndPointProvisioner
    {
        [RequiresIpEndPoint]
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
