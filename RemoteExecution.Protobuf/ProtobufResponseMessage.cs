using System;
using System.IO;
using System.Reflection;
using ProtoBuf;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution
{
    [ProtoContract]
    internal class ProtobufResponseMessage : IResponseMessage, IIncomplete
    {
        [ProtoMember(1)]
        public string CorrelationId { get; set; }

        public string MessageType
        {
            get { return CorrelationId; }
            set
            {
                throw new InvalidOperationException(
                    "Cannot set MessageType on ProtobufResponseMessage- this just echos the CorrelationId.");
            }
        }

        public object Value { get; set; }

        [ProtoMember(2)]
        public byte[] SerializableValue { get; set; }

        public void Complete(MethodInfo info)
        {
            if (info.ReturnType != typeof(void))
                using (MemoryStream stream = new MemoryStream(SerializableValue))
                    Value = Serializer.Deserialize(info.ReturnType, stream);
        }
    }
}