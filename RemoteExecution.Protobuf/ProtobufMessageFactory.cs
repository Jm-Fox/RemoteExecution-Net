using System;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution
{
    /// <summary>
    /// Message factory that provides Protobufs (method parameters should be protobufs too).
    /// </summary>
    public class ProtobufMessageFactory : IMessageFactory
    {
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
            return new ProtobufRequestMessage
            {
                CorrelationId = id,
                Args = args,
                MessageType = interfaceName,
                MethodName = methodName
            };
        }

        /// <summary>
        /// Creates an IResponseMessage.
        /// </summary>
        /// <param name="id">Correlation id</param>
        /// <param name="value">Method result</param>
        /// <returns>IResponseMessage for use</returns>
        public IResponseMessage CreateResponseMessage(string id, object value)
        {
            return new ProtobufResponseMessage
            {
                CorrelationId = id,
                Value = value
            };
        }

        /// <summary>
        /// Creates an IResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IResponseMessage for use</returns>
        public IResponseMessage CreateResponseMessage()
        {
            return new ProtobufResponseMessage();
        }

        /// <summary>
        /// Creates an IExceptionResponseMessage using the default constructor.
        /// </summary>
        /// <returns>IExceptionResponseMessage for use</returns>
        public IExceptionResponseMessage CreateExceptionResponseMessage()
        {
            return new ProtobufExceptionResponseMessage();
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
            return new ProtobufExceptionResponseMessage
            {
                CorrelationId = id,
                ExceptionType = exceptionType.AssemblyQualifiedName,
                Message = message
            };
        }
    }
}