using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution
{
    [ProtoContract]
    internal class ProtobufRequestMessage : IRequestMessage, IIncomplete
    {
        [ProtoMember(1)]
        public string CorrelationId { get; set; }

        [ProtoMember(2)]
        public string MessageType { get; set; }

        [ProtoMember(3)]
        public string MethodName { get; set; }

        public object[] Args { get; set; }

        [ProtoMember(4)]
        public byte[] SerializableArgs { get; set; }
        
        public bool IsResponseExpected { get; set; }

        public IOutputChannel Channel { get; set; }

        private static readonly RuntimeTypeModel Model = RuntimeTypeModel.Default;

        // see https://github.com/mgravell/protobuf-net/issues/212
        private static readonly MethodInfo MapType =
            Model.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == "MapType" && m.GetParameters().Length == 1);

        public void Complete(MethodInfo info)
        {
            IsResponseExpected = info.ReturnType != typeof(void);
            ParameterInfo[] infos = info.GetParameters();
            Args = new object[infos.Length];
            using (MemoryStream stream = new MemoryStream(SerializableArgs))
                for (int i = 0; i < Args.Length; i++) {
                    Args[i] = Model.DeserializeWithLengthPrefix(stream, null,
                        (Type)MapType.Invoke(Model, new object[] { infos[i].ParameterType }), PrefixStyle.Fixed32, 0);
                }
        }
    }
}