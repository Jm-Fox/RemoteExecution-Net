using System;
using RemoteExecution.Remoting;

namespace RemoteExecution.TransportLayer.Lidgren.Wrappers.IT.MS
{
    public interface IWeakCalculator
    {
        [OneWay]
        void Destroy();

        int Add(int x, int y);
    }

    class WeakCalculator : IWeakCalculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }

        public Action Destroyed;

        public void Destroy()
        {
            Destroyed();
        }
    }
}
