using RemoteExecution.Connections;
using System;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;
using RemoteExecution.TransportLayer;
using SharedClientServices.Contracts;

namespace SharedClientServices.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SharedConfigurator.Configure();

            using (var client1 = CreateSharedConnection("shareme", "net://localhost:3131/SharedAppId"))
            using (var client2 = CreateSharedConnection("shareme", "net://localhost:3132/SharedAppId"))
            {
                client1.Open();
                client2.Open();

                IBasicService service1 = client1.RemoteExecutor.Create<IBasicService>();
                IBasicService service2 = client2.RemoteExecutor.Create<IBasicService>();

                Console.WriteLine($"Service 1 says that 2 + 2 = {service1.Add(2, 2)}");
                Console.WriteLine($"Service 2 says that 2 + 2 = {service2.Add(2, 2)}");

                Console.WriteLine("Saying hello to service1...");
                service1.SayHello("Hi!");
                Console.WriteLine("Saying hello to service2...");
                service2.SayHello("Sup!");

                Console.WriteLine("Done. Press enter to exit.");
                Console.ReadLine();
            }
        }

        private static readonly OperationDispatcher dispatcher = new OperationDispatcher();

        private static ClientConnection CreateSharedConnection(string clientId, string address)
        {
            return new ClientConnection(TransportLayerResolver.CreateClientChannelFor(clientId, new Uri(address)),
                dispatcher, new ConnectionConfig());
        }
    }
}
