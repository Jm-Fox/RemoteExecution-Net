using EncryptedServices.Contracts;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace EncryptedServices.Server
{
	class Host : StatefulServerEndpoint
	{
		public Host(string uri)
			: base(uri, new ServerConfig())
		{
		}

		protected override void InitializeConnection(IRemoteConnection connection)
		{
			var clientCallback = connection.RemoteExecutor.Create<ISessionEncryptedCallback>();
			connection.OperationDispatcher.RegisterHandler<IAuthenticator>(new Authenticator(clientCallback));
		}
	}
}