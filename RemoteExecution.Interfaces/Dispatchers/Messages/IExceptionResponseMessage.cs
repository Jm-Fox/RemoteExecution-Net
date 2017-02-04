namespace RemoteExecution.Dispatchers.Messages
{
    /// <summary>
    /// Response message interface containing an exception.
    /// </summary>
    public interface IExceptionResponseMessage : IResponseMessage {
        /// <summary>
        /// Type of exception.
        /// </summary>
        string ExceptionType { get; set; }

        /// <summary>
        /// Exception message.
        /// </summary>
        string Message { get; set; }
    }
}