using System;
using System.Threading;

namespace RemoteExecution.Dispatchers.Handlers
{
	/// <summary>
	/// Interface for response handler.
	/// </summary>
	public interface IResponseHandler : IMessageHandler
	{
        /// <summary>
        /// Indicates whether or not GetValue will throw a NullReferenceException (opposite)
        /// </summary>
	    bool HasValue { get; }

	    /// <summary>
		/// Returns response value.
		/// It should be called after WaitForResponse() returns.
		/// </summary>
		/// <returns></returns>
		object GetValue();

        /// <summary>
        /// Blocks until response is handled.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        void WaitForResponse(TimeSpan timeout, CancellationToken cancellationToken);
	}
}