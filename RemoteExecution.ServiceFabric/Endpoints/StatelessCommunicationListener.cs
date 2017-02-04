using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;

namespace RemoteExecution.ServiceFabric.Endpoints
{
	/// <summary>
	/// ICommunicationListener implementation of StatelessServerEndpoin.
	/// </summary>
    public class StatelessCommunicationListener : StatelessServerEndpoint, ICommunicationListener
    {
        /// <summary>
        /// Host address (loopback or whoami :port)
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Hosts on the given uri.
        /// </summary>
        /// <param name="uri"></param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
        public StatelessCommunicationListener(string uri, IOperationDispatcher dispatcher)
            : base(uri, dispatcher)
        {
            Uri = uri;
        }

        /// <summary>
        /// Builds the stateless communication listener from the service manifest's endpoint.
        /// </summary>
        /// <param name="context">ServiceFabric's context</param>
        /// <param name="endPointListenerName">Name of endpoint in service manifest</param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
        public StatelessCommunicationListener(ServiceContext context, string endPointListenerName,
            IOperationDispatcher dispatcher)
            : this(CommonHelper.GetUriFromContext(context, endPointListenerName), dispatcher)
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
            if (this.GetPort() != uri.Port)
            {
                Uri = $"{uri.Scheme}://{uri.Authority}:{this.GetPort()}{uri.AbsolutePath}";
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
    }
}