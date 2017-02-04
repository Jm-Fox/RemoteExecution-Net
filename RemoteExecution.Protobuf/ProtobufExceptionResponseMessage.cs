using System;
using ProtoBuf;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution
{
    [ProtoContract]
    internal class ProtobufExceptionResponseMessage : IExceptionResponseMessage
    {
        [ProtoMember(1)]
        public string CorrelationId { get; set; }
        
        [ProtoMember(2)]
        public string MessageType
        {
            get { return CorrelationId; }
            set { CorrelationId = value; }
        }
        
        public object Value
        {
            get { throw (Exception) Activator.CreateInstance(Type.GetType(ExceptionType, true), Message); }
            set { throw new InvalidOperationException(); }
        }
        
        [ProtoMember(3)]
        public string ExceptionType { get; set; }
        
        [ProtoMember(4)]
        public string Message { get; set; }
    }
}