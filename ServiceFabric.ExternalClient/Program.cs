using System;
using System.Threading;
using RemoteExecution.Channels;
using RemoteExecution.Connections;
using ServiceFabric.Contracts;

namespace ServiceFabric.ExternalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // todo: durable
            DurableConfigurator.Configure();
            // note that at most one process can use a single UDP port on a machine.
            // Because of this, the service fabric services exposed to the client via this port cannot be replicated
            // on local.1node
            // If you deploy to an actual service fabric cluster, then the service can be replicated on several machines
            // and reached via the load balancer. This means that if a service has a high spin up cost, your reconnection
            // time is just a combination of the timeout and the load balancer's health ping.
            // Note that you will want to have the load balancer hash on client IP (hashing on client port is not required)
            //
            // However, the ServiceFabric.Stateless executable is only reachable via the naming service, so it doesn't
            // require a dedicated port. This means that when using local.5node, there will be 5 stateless executables.
            // Thus replacing this internal client is virtually free, since it just takes talking to the naming service.
            using (var strongCalculator = new DurableClientConnection("net://localhost:3232/StrongCalculatorApplicationId"))
            using (var weakCounter = new DurableClientConnection("net://localhost:3233/WeakCounterApplicationId"))
            {
                //strongCalculator.Open();
                weakCounter.Open();
                strongCalculator.ConnectionAborted += () => Console.WriteLine("\tStrong calculator connection aborted.");
                strongCalculator.ConnectionInterrupted += () => Console.WriteLine("\tStrong calculator connection interrupted.");
                strongCalculator.ConnectionRestored += () => Console.WriteLine("\tStrong calculator connection restored.");
                weakCounter.ConnectionAborted += () => Console.WriteLine("\tWeak counter connection aborted.");
                weakCounter.ConnectionInterrupted += () => Console.WriteLine("\tWeak counter connection interrupted.");
                weakCounter.ConnectionRestored += () => Console.WriteLine("\tWeak counter connection restored.");
                //var calculator = strongCalculator.RemoteExecutor.Create<IWeakCalculator>();
                var counter = weakCounter.RemoteExecutor.Create<IWeakCounter>();
                Console.WriteLine("Connected to StrongCalculator and WeakCounter.");

                //Console.WriteLine("4 + 5 = " + calculator.Add(4, 5));
                //Console.WriteLine("Crashing calculator");
                //calculator.Crash();
                //Console.WriteLine("9 + 5 = " + calculator.Add(9, 5));
                Console.WriteLine("Incrementing...");
                counter.Increment();
                Console.WriteLine("Incrementing...");
                counter.Increment();
                Console.WriteLine("Incrementing...");
                counter.Increment();
                Console.WriteLine("Incrementing...");
                counter.Increment();
                Console.WriteLine("Count: " + counter.GetCount());
                Console.WriteLine("Crashing counter...");
                counter.Crash();
                // Wait a short while to ensure crash
                Console.WriteLine("Sleeping to ensure service crashes before next call");
                Thread.Sleep(1500);
                //Console.WriteLine("3 + 2 = " + calculator.Add(3, 2));
                Console.WriteLine("Incrementing...");
                try
                {
                    counter.Increment();
                    Console.WriteLine("Count: " + counter.GetCount());
                }
                catch (ConnectionOpenException)
                {
                    Console.WriteLine("Connection was not re-established soon enough.");
                    Console.WriteLine("Consider increasing timeout, retry attempts, or ");
                    Console.WriteLine("decreasing time until the service comes back online.");
                    Console.WriteLine("Done. Press enter to exit.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Count was not persisted because example service does not use persistence.");
                Console.WriteLine("Should persistence be applied (ReliableCollections) or otherwise,");
                Console.WriteLine("then the client would not see any issues.");

                Console.WriteLine("Done. Press enter to exit.");
                Console.ReadLine();
            }
        }
    }
}