using System;

namespace RemoteExecution.Dispatchers.Messages
{
	[Serializable]
	internal class ExceptionResponseMessage : IExceptionResponseMessage
	{
		public string ExceptionType { get; set; }
		public string Message { get; set; }

		public ExceptionResponseMessage()
		{
		}

		public ExceptionResponseMessage(string id, Type exceptionType, string message)
		{
			ExceptionType = exceptionType.AssemblyQualifiedName;
			Message = message;
			CorrelationId = id;
		}

		#region IResponseMessage Members

		public object Value
		{
			get { throw (Exception)Activator.CreateInstance(Type.GetType(ExceptionType, true), Message); }
		}

		public string CorrelationId { get; set; }
		public string MessageType { get { return CorrelationId; } }

		#endregion
	}
}