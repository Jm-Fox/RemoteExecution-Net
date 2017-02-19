using System;
using RemoteExecution.Connections;
using ServiceFabric.Contracts;

namespace ServiceFabric.ExternalClient
{
    class Program
    {
        static void Main(string[] args)
        {
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
            using (var strongCalculator = new DurableClientConnection("net://localhost:3232/StrongCalculator"))
            using (var weakCounter = new DurableClientConnection("net://localhost:3233/WeakCounter"))
            {
                strongCalculator.ConnectionAborted += () => Console.WriteLine("\tStrong calculator connection aborted.");
                strongCalculator.ConnectionInterrupted += () => Console.WriteLine("\tStrong calculator connection interrupted.");
                strongCalculator.ConnectionRestored += () => Console.WriteLine("\tStrong calculator connection restored.");
                weakCounter.ConnectionAborted += () => Console.WriteLine("\tWeak counter connection aborted.");
                weakCounter.ConnectionInterrupted += () => Console.WriteLine("\tWeak counter connection interrupted.");
                weakCounter.ConnectionRestored += () => Console.WriteLine("\tWeak counter connection restored.");
                strongCalculator.Open();
                weakCounter.Open();
                var calculator = strongCalculator.RemoteExecutor.Create<IWeakCalculator>();
                var counter = weakCounter.RemoteExecutor.Create<IWeakCounter>();
                Console.WriteLine("Connected to StrongCalculator and WeakCounter.");

                Console.WriteLine("4 + 5 = " + calculator.Add(4, 5));
                Console.WriteLine("Crashing calculator");
                calculator.Crash();
                Console.WriteLine("9 + 5 = " + calculator.Add(9, 5));
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
                Console.WriteLine("3 + 2 = " + calculator.Add(3, 2));
                Console.WriteLine("Incrementing...");
                counter.Increment();
                Console.WriteLine("Count: " + counter.GetCount());

                Console.WriteLine("Done. Press enter to exit.");
                Console.ReadLine();
            }
        }
    }
}