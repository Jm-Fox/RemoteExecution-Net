using System;

namespace RemoteExecution.Dispatchers.Messages {
    /// <summary>
    /// Creates messages provided in the base RemoteExecution.Core implementation
    /// </summary>
    public class DefaultMessageFactory : IMessageFactory {
        /// <summary>
        /// Creates an IRequestMessage.
        /// </summary>
        /// <param name="id">Correlation identifier</param>
        /// <param name="interfaceName">Name of interface to use</param>
        /// <param name="methodName">Name of method to invoke</param>
        /// <param name="args">Arguments of method invocation</param>
        /// <param name="isResponseExpected">Whether or not a response is expected</param>
        /// <returns>IRequestMessage for use</returns>
        public IRequestMessage CreateRequestMessage(string id, string interfaceName, string methodName, object[] args,
            bool isResponseExpected)
        {
            return new RequestMessage(id, interfaceName, methodName, args, isResponseExpected);
        }

        /// <summary>
        /// Creates an IResponseMessage.
        /// </summary>
        /// <param name="id">Correlation id</param>
        /// <param name="value">Method result</param>
        /// <returns>IResponseMessage for use</returns>
        public IResponseMessage CreateResponseMessage(string id, object value)
        {
            return new ResponseMessage(id, value);
        }

        /// <summary>
        /// Creates an IResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IResponseMessage for use</returns>
        public IResponseMessage CreateResponseMessage()
        {
            return new ResponseMessage();
        }

        /// <summary>
        /// Creates an IExceptionResponseMessage.
        /// </summary>
        /// <param name="id">Correlation id</param>
        /// <param name="exceptionType">Type of exception being thrown</param>
        /// <param name="message">Exception message</param>
        /// <returns>IExceptionResponseMessage for use</returns>
        public IExceptionResponseMessage CreateExceptionResponseMessage(string id, Type exceptionType, string message)
        {
            return new ExceptionResponseMessage(id, exceptionType, message);
        }

        /// <summary>
        /// Creates an IExceptionResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IExceptionResponseMessage for use</returns>
        public IExceptionResponseMessage CreateExceptionResponseMessage()
        {
            return new ExceptionResponseMessage();
        }
    }
}
