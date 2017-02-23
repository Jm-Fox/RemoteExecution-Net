using System;
using RemoteExecution.ServiceFabric.Connections;
using ServiceFabric.Contracts;

namespace ServiceFabric.Stateful
{
    class StrongCalculator : IWeakCalculator
    {
        private readonly IWeakCalculator remoteWeakCalculator;

        public StrongCalculator()
        {
            FabricClientConnection connection = new FabricClientConnection(new Uri("fabric:/ServiceFabric.App/Stateless"));
            connection.Open();
            remoteWeakCalculator = connection.RemoteExecutor.Create<IWeakCalculator>();
        }

        public void Crash()
        {
            remoteWeakCalculator.Crash();
        }

        public int Add(int x, int y)
        {
            return remoteWeakCalculator.Add(x, y);
        }
    }
}