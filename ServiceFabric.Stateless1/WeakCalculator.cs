using System;
using ServiceFabric.Contracts;

namespace ServiceFabric.Stateless
{
    class WeakCalculator : IWeakCalculator
    {
        public Action Crashed;

        public void Crash()
        {
            Crashed?.Invoke();
        }

        public int Add(int x, int y)
        {
            Console.WriteLine($"\tAdding {x} + {y} = {x + y}");
            return x + y;
        }
    }
}
