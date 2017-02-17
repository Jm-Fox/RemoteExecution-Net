using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using SharedClientServices.Contracts;
using System;

namespace SharedClientServices.Server1
{
    class Program
    {
        static void Main(string[] args)
        {
            SharedConfigurator.Configure();

            var dispatcher = new OperationDispatcher()
                .RegisterHandler<IBasicService>(new BasicService());


            using (var host = new StatelessServerEndpoint("net://127.0.0.1:3131/SharedAppId", dispatcher))
            {
                host.Start();
                Console.WriteLine("Server started...\nPress enter to stop");
                Console.ReadLine();
            }
        }
    }
}
