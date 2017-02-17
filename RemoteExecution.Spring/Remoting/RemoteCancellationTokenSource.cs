using System.Threading;

namespace RemoteExecution.Remoting
{
    internal class RemoteCancellationTokenSource : CancellationTokenSource
    {
        public bool Aborted = false;
        public bool Restored = false;
    }
}
