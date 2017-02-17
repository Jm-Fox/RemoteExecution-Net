using System;
using System.Threading;
using System.Threading.Tasks;
using DurableServices.Contracts;
using RemoteExecution.Connections;

namespace DurableServices.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            DurableConfigurator.Configure();
            using (var client = new DurableClientConnection("net://localhost:3133/DurableServices"))
            {
                bool stop = false;
                bool wait = false;
                client.ConnectionPaused = response => Console.WriteLine("\tConnection lost; attempting to reconnect...");
                client.ConnectionRestored += () => Console.WriteLine("\tConnection has been restored.");
                client.ConnectionAborted += () =>
                {
                    Console.WriteLine("\tConnection has been aborted.");
                    wait = true;
                    stop = true;
                };
                client.Closed += () => Console.WriteLine("\tConnection has been closed.");
                client.Open();
                ICalculator calculator = client.RemoteExecutor.Create<ICalculator>();

                Console.WriteLine("Sending periodic requests. Press enter to exit.");
                Task.Run(() =>
                {
                    Console.ReadLine();
                    stop = true;
                });
                while (!stop)
                {
                    int sleep = R() % 1000 + 300;
                    Task.Run(() => AsyncAdd(R() % 100, R() % 100, calculator));
                    Thread.Sleep(sleep);
                }
                if (wait)
                {
                    Console.WriteLine("\tPress enter to exit.");
                    Console.ReadLine();
                }
            }
        }

        private static readonly Random random = new Random();

        private static int R()
        {
            return random.Next();
        }

        private static void AsyncAdd(int x, int y, ICalculator calculator)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Thread {threadId}: Adding {x} and {y}");
            try
            {
                Console.WriteLine($"Thread {threadId}: {x} + {y} = {calculator.Add(x, y)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Thread {threadId}: Exception thrown ({ex.GetType().Name})");
            }
        }
    }
}
