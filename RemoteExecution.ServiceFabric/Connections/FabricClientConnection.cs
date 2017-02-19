using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;

namespace RemoteExecution.ServiceFabric.Connections
{
    // FabricClientConnection's errors are not self contained. If it
    // cannot automatically repair the connection, exceptions will be thrown and
    // the caller will need to address the issue.
    /// <summary>
    /// Provides a connection to a service fabric service (client must be inside the cluster)
    /// </summary>
    public class FabricClientConnection : IClientConnection
    {
        /// <summary>
        /// [Singular] event fired when the connection is closed. The listener can suggest a new host and port where the service might be located.
        /// If not used, reconnection attempts will hit the same host and port.
        /// </summary>
        public Action<ClosedConnectionResponse> ConnectionPaused
        {
            get { return clientConnection.ConnectionPaused; }
            set { clientConnection.ConnectionPaused = value; }
        }

        /// <summary>
        /// Event fired when connection is restored.
        /// </summary>
        public event Action ConnectionRestored
        {
            add { clientConnection.ConnectionRestored += value; }
            remove { clientConnection.ConnectionRestored -= value; }
        }

        /// <summary>
        /// Event fired when connection is aborted.
        /// </summary>
        public event Action ConnectionAborted
        {
            add { clientConnection.ConnectionAborted += value; }
            remove { clientConnection.ConnectionAborted -= value; }
        }

        /// <summary>
        /// Event fired when connection is down.
        /// </summary>
        public event Action ConnectionInterrupted
        {
            add { clientConnection.ConnectionInterrupted += value; }
            remove { clientConnection.ConnectionInterrupted -= value; }
        }

        /// <summary>
        /// How many times the channel can try to reconnect before aborting.
        /// </summary>
        public int RetryAttempts
        {
            get { return clientConnection.RetryAttempts; }
            set { clientConnection.RetryAttempts = value; }
        }

        private static readonly Random Random = new Random();
        private readonly DurableClientConnection clientConnection;
        private ResolvedServiceEndpoint selectedEndpoint;
        /// <summary>
        /// Providse the resolved address of the service
        /// </summary>
        public Uri FabricAddress { get; }
        /// <summary>
        /// Service Partition Resolver.
        /// </summary>
        public ServicePartitionResolver Resolver;

        /// <summary>
        /// Event fired when the connection is finally closed (todo: finish implementing this)
        /// </summary>
        public event Action Closed;

        /// <summary>
        /// Creates a client connection for the specified fabric address (beginning with fabric:/)
        /// </summary>
        /// <param name="fabricAddress"></param>
        public FabricClientConnection(Uri fabricAddress)
            : this(fabricAddress, ServicePartitionResolver.GetDefault())
        {
            // todo: remove (here temporarily to stop compiler warning/error)
            Closed?.Invoke();
        }

        /// <summary>
        /// Creates a client connection for the specified fabric address (beginning with fabric:/)
        /// </summary>
        /// <param name="fabricAddress"></param>
        /// <param name="resolver">Non-default ServicePartitionResolver</param>
        /// todo: make sure custom partitioning works (and in the process learn how custom partitioning works)
        public FabricClientConnection(Uri fabricAddress, ServicePartitionResolver resolver)
        {
            FabricAddress = fabricAddress;
            Resolver = resolver;

            selectedEndpoint = ResolveAnyEndpoint().Result;
            
            clientConnection = new DurableClientConnection(selectedEndpoint.Address);
            clientConnection.ConnectionPaused = OnConnectionPaused;
        }

        private void OnConnectionPaused(ClosedConnectionResponse response)
        {
            if (IsSelectedPartitionDown())
            {
                selectedEndpoint = ResolveAnyEndpoint().Result;
                Uri uri = new Uri(selectedEndpoint.Address);
                response.ReconnectHost = uri.Host;
                response.ReconnectPort = (ushort)uri.Port;
            }
        }

        private async Task<ResolvedServiceEndpoint> ResolveAnyEndpoint()
        {
            int random = Random.Next();
            ResolvedServicePartition resolved =  await Resolver.ResolveAsync(FabricAddress, new ServicePartitionKey(), CancellationToken.None);
            ResolvedServiceEndpoint[] array = resolved.Endpoints.Where(
                    e => e.Role == ServiceEndpointRole.StatefulPrimary || e.Role == ServiceEndpointRole.Stateless)
                .ToArray();
            if (array.Length == 0)
                throw new FabricServiceNotFoundException("No live primary services found (name may or may not be correct)");
            return array[array.Length % random];
        }

        /// <summary>
        /// Note that this involves making a TCP call to a service possibly on another machine in the cluster. It is not fast.
        /// </summary>
        private bool IsSelectedPartitionDown()
        {
            ResolvedServicePartition resolved =
                Resolver.ResolveAsync(FabricAddress, new ServicePartitionKey(), CancellationToken.None).Result;
            return resolved.Endpoints.Contains(selectedEndpoint);
        }

        /// <summary>
        /// Returns true if connection is opened, otherwise false.
        /// </summary>
        public bool IsOpen => clientConnection.IsOpen;

        /// <summary>
        /// Returns operation dispatcher used for handling incoming operations.
        /// </summary>
        public IOperationDispatcher OperationDispatcher => clientConnection.OperationDispatcher;

        /// <summary>
        /// Returns remote executor used for executing operations on remote end.
        /// </summary>
        public IRemoteExecutor RemoteExecutor => clientConnection.RemoteExecutor;

        /// <summary>
        /// Disposes the FabricClientConnection.
        /// </summary>
        public void Dispose()
        {
            clientConnection.Dispose();
        }

        /// <summary>
        /// Opens connection and make it ready for sending and handling operation requests.
        /// If connection has been closed, this method reopens it.
        /// </summary>
        public void Open()
        {
            clientConnection.Open();
        }
    }
}
