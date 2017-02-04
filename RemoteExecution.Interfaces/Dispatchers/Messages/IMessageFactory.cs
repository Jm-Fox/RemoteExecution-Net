using System;

namespace RemoteExecution.Dispatchers.Messages {
    /// <summary>
    /// Provides flexible message instancing for flexible serialization.
    /// </summary>
    public interface IMessageFactory {
        /// <summary>
        /// Creates an IRequestMessage.
        /// </summary>
        /// <param name="id">Correlation identifier</param>
        /// <param name="interfaceName">Name of interface to use</param>
        /// <param name="methodName">Name of method to invoke</param>
        /// <param name="args">Arguments of method invocation</param>
        /// <param name="isResponseExpected">Whether or not a response is expected</param>
        /// <returns>IRequestMessage for use</returns>
        IRequestMessage CreateRequestMessage(string id, string interfaceName, string methodName, object[] args,
            bool isResponseExpected);

        /// <summary>
        /// Creates an IResponseMessage.
        /// </summary>
        /// <param name="id">Correlation id</param>
        /// <param name="value">Method result</param>
        /// <returns>IResponseMessage for use</returns>
        IResponseMessage CreateResponseMessage(string id, object value);

        /// <summary>
        /// Creates an IResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IResponseMessage for use</returns>
        IResponseMessage CreateResponseMessage();

        /// <summary>
        /// Creates an IExceptionResponseMessage.
        /// </summary>
        /// <param name="id">Correlation id</param>
        /// <param name="exceptionType">Type of exception being thrown</param>
        /// <param name="message">Exception message</param>
        /// <returns>IExceptionResponseMessage for use</returns>
        IExceptionResponseMessage CreateExceptionResponseMessage(string id, Type exceptionType, string message);

        /// <summary>
        /// Creates an IExceptionResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IExceptionResponseMessage for use</returns>
        IExceptionResponseMessage CreateExceptionResponseMessage();
    }
}
