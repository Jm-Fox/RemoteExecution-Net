using System;
using DurableServices.Contracts;

namespace DurableServices.Server
{
    public class Calculator : ICalculator
    {
        public int Add(int x, int y)
        {
            Console.WriteLine(string.Format("\t {0} + {1} = {2}", x, y, x + y));
            return x + y;
        }
    }
}
