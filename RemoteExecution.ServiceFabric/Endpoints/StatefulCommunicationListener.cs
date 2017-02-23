using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace RemoteExecution.ServiceFabric.Endpoints
{
    /// <summary>
    /// ICommunicationListener implementation of StatefulServerEndpoint.
    /// </summary>
    public class StatefulCommunicationListener : StatefulServerEndpoint, ICommunicationListener
    {
        /// <summary>
        /// Action for service initialization.
        /// </summary>
        public Action<IRemoteConnection> Initialized;

        /// <summary>
        /// Host address (loopback or whoami :port)
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Hosts on the given uri.
        /// </summary>
        /// <param name="uri"></param>
        public StatefulCommunicationListener(string uri)
            : this(uri, null)
        {
        }

        /// <summary>
        /// Hosts on the given uri with action to take when initialized.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="initialized">Action to be performed upon initialization (IRemoteConnection is not accessible until this point)</param>
        public StatefulCommunicationListener(string uri, Action<IRemoteConnection> initialized)
            : base(uri, new ServerConfig())
        {
            Uri = uri;
            Initialized = initialized;
        }

        /// <summary>
        /// Builds the stateful communication listener from the service manifest's endpoint.
        /// </summary>
        /// todo: Make path in uri {scheme}:/host:port/{path} not be forced to be the endpoint's name
        /// <param name="context">ServiceFabric's context</param>
        /// <param name="endPointListenerName">Name of endpoint in service manifest</param>
        /// <param name="initialized">Action to be performed upon initialization (IRemoteConnection is not accessible until this point)</param>
        public StatefulCommunicationListener(ServiceContext context, string endPointListenerName, Action<IRemoteConnection> initialized)
            : this(context.GetEndpoint(endPointListenerName), initialized)
        {
        }

        /// <summary>
        /// Opens the ICommunicationListener.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Start();
            Uri uri = new Uri(Uri);
            if (Port != uri.Port)
            {
                Uri = $"{uri.Scheme}://{uri.Authority}:{Port}{uri.AbsolutePath}";
            }
            return Task.FromResult(Uri);
        }

        /// <summary>
        /// Closes the ICommunicationListener.
        /// </summary>
        /// todo: determine whether or not to allow re-opening
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return new Task(Dispose);
        }

        /// <summary>
        /// Aborts the ICommunicationListener.
        /// </summary>
        public void Abort()
        {
            Dispose();
        }

        /// <summary>
        /// Fires the Initialized event.
        /// </summary>
        /// <param name="connection"></param>
        protected override void InitializeConnection(IRemoteConnection connection)
        {
            Initialized?.Invoke(connection);
        }
    }
}