using System;
using System.Reflection;
using System.Threading;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Dispatchers.Handlers {
    /// <summary>
    /// Basic response handler.
    /// </summary>
	public class ResponseHandler : IResponseHandler, IIncomplete {
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
        private IResponseMessage _response;

        /// <summary>
        /// Assigns identifier for handler group
        /// </summary>
        /// <param name="handlerGroupId">Handler group identifier</param>
		public ResponseHandler(Guid handlerGroupId)
        {
            HandlerGroupId = handlerGroupId;
            HandledMessageType = Guid.NewGuid().ToString();
        }

        #region IResponseHandler Members

        /// <summary>
        /// Indicates whether or not GetValue will throw a NullReferenceException (opposite)
        /// </summary>
        public bool HasValue => _response != null;

        /// <summary>
        /// Type of messages handled.
        /// </summary>
        public string HandledMessageType { get; private set; }

        /// <summary>
        /// Handler group identifier.
        /// </summary>
		public Guid HandlerGroupId { get; private set; }

        /// <summary>
        /// Accepts message.
        /// </summary>
        /// <param name="msg"></param>
		public void Handle(IMessage msg)
        {
            _response = ((IResponseMessage)msg);
            _resetEvent.Set();
        }

        /// <summary>
        /// Gets value from the response.
        /// </summary>
        /// <returns></returns>
		public object GetValue()
        {
            return _response.Value;
        }

        /// <summary>
        /// Waits until a response arrives.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
		public void WaitForResponse(TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                _resetEvent.Wait(timeout, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Completes the response, if it's incomplete
        /// </summary>
        /// <param name="info">MethodInfo of the method that was invoked</param>
	    public void Complete(MethodInfo info)
        {
            (_response as IIncomplete)?.Complete(info);
        }

        #endregion
    }
}