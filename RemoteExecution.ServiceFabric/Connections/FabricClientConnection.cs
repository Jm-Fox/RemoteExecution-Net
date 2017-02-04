﻿using System;
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
        private static readonly Random Random = new Random();
        private readonly ClientConnection clientConnection;
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

            clientConnection = new ClientConnection(selectedEndpoint.Address);
            clientConnection.SetConnectionClosed(ConnectionClosed);
        }

        private ClosedConnectionResponse ConnectionClosed()
        {
            if (SelectedPartitionDown)
            {
                selectedEndpoint = ResolveAnyEndpoint().Result;
                Uri uri = new Uri(selectedEndpoint.Address);
                return new ClosedConnectionResponse
                {
                    Fail = false,
                    ReconnectHost = uri.Host,
                    ReconnectPort = (ushort)uri.Port
                };
            }
            return null;
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

        private bool SelectedPartitionDown
        {
            get
            {
                ResolvedServicePartition resolved =
                    Resolver.ResolveAsync(FabricAddress, new ServicePartitionKey(), CancellationToken.None).Result;
                return resolved.Endpoints.Contains(selectedEndpoint);
            }
        }

        /// <summary>
        /// Returns true if connection is opened, otherwise false.
        /// </summary>
        /// todo: double check this won't cause failures during attempted reconnects
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
