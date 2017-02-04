using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	/// <summary>
	/// Server configuration class allowing to configure server dependencies as well as specify maximum number of allowed connections.
	/// </summary>
	public class ServerConfig : IServerConfig
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ServerConfig()
		{
			MaxConnections = 128;
			RemoteExecutorFactory = DefaultConfig.RemoteExecutorFactory;
			TaskScheduler = DefaultConfig.TaskScheduler;
		}

		#region IServerConfig Members

		/// <summary>
		/// Returns remote executor factory that should be used by client.
		/// Default value is taken from <see cref="DefaultConfig"/>.
		/// </summary>
		public IRemoteExecutorFactory RemoteExecutorFactory { get; set; }

		/// <summary>
		/// Returns task scheduler that should be used by client.
		/// Default value is taken from <see cref="DefaultConfig"/>.
		/// </summary>
		public ITaskScheduler TaskScheduler { get; set; }

		/// <summary>
		/// Maximum connections number that would be allowed by ServerEndpoint.
		/// Default is 128.
		/// </summary>
		public int MaxConnections { get; set; }

		#endregion
	}
}