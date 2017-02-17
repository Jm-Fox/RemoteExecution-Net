using System;
using System.Threading;
using DurableServices.Contracts;

namespace DurableServices.Server
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            DurableConfigurator.Configure();
            Host host = new Host("net://127.0.0.1:3133/DurableServices");
            Greet();
            while (true)
            {
                host.Start();
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }
                int x;
                try
                {
                    x = Convert.ToInt32(input);
                }
                catch
                {
                    Console.WriteLine("Conversion to int32 failed.");
                    continue;
                }
                Console.WriteLine("Server stopped.");
                host.Dispose();
                Thread.Sleep(x);
                host = new Host("net://127.0.0.1:3133/DurableServices");
                Greet();
            }
            host.Dispose();
        }

        private static void Greet()
        {
            Console.WriteLine("Server started...\nPress enter to stop");
            Console.WriteLine(
                "OR enter an integer x. The server will be destroyed and restored after x milliseconds.");
        }
    }
}
