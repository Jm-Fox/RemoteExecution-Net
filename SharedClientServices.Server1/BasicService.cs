using System;
using System.Net;
using SharedClientServices.Contracts;

namespace SharedClientServices.Server1
{
    internal class BasicService : IBasicService
    {
        public int Add(int x, int y)
        {
            return x - y;
        }

        public void SayHello(string message, IPEndPoint endPoint = null)
        {
            Console.WriteLine($"1 | {endPoint} says " + message);
        }
    }
}
