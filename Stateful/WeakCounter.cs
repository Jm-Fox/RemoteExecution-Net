using ServiceFabric.Contracts;
using System;

namespace ServiceFabric.Stateful
{
    public class WeakCounter : IWeakCounter
    {
        private int count;
        private readonly object sync = new object();

        public Action Crashed;

        public void Crash()
        {
            Crashed?.Invoke();
        }

        public void Increment()
        {
            lock (sync)
                count++;
        }

        public int GetCount()
        {
            lock (sync)
                return count;
        }
    }
}