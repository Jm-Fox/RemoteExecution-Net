using DurableServices.Contracts;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;

namespace DurableServices.Server
{
	internal class Host : StatelessServerEndpoint
	{
		public Host(string uri)
			: base(uri, new OperationDispatcher().RegisterHandler<ICalculator>(new Calculator()))
		{
		}
	}
}