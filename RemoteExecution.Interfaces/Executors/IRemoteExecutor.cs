namespace RemoteExecution.Executors
{
	/// <summary>
	/// Remote executor interface allowing to create proxy objects, that would execute all of its methods remotely.
	/// </summary>
	public interface IRemoteExecutor
	{
		/// <summary>
		/// Creates proxy object for interface <c>T</c>, that would execute all of its methods remotely.
		/// All methods of interface are treated as two way methods.
		/// </summary>
		/// <typeparam name="T">Type of proxy to create.</typeparam>
		/// <returns>Proxy object.</returns>
		T Create<T>();

		/// <summary>
		/// Creates proxy object for interface <c>T</c>, that would execute all of its methods remotely.
		/// Methods returning result are always treated as two way methods, while no result methods treatment depends on <c>returnPolicy</c> parameter.
		/// </summary>
		/// <typeparam name="T">Type of proxy to create.</typeparam>
		/// <param name="returnPolicy">Specifies if no result methods (returning void) are one or two way.</param>
		/// <returns>Proxy object.</returns>
		T Create<T>(ReturnPolicy returnPolicy);
	}
}