using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.Serializers;

namespace RemoteExecution
{
    /// <summary>
    /// Class for the serialization of protobufs.
    /// </summary>
    public class ProtobufSerializer : IMessageSerializer
    {
        private static readonly Dictionary<Type, byte> TypeToByte = new Dictionary<Type, byte>
        {
            {typeof(ProtobufRequestMessage), 0},
            {typeof(ProtobufResponseMessage), 1},
            {typeof(ProtobufExceptionResponseMessage), 2}
        };

        /// <summary>
        /// Deserializes byte array into message object.
        /// </summary>
        /// <param name="msg">Serialized message.</param>
        /// <returns>Deserialized message. Message may be incomplete and require further information to completely deserialize.</returns>
        public IMessage Deserialize(byte[] msg)
        {
            MemoryStream stream = new MemoryStream(msg);
            switch (stream.ReadByte())
            {
                // Request message
                case 0:
                    return Serializer.Deserialize<ProtobufRequestMessage>(stream);
                // Response message
                case 1:
                    return Serializer.Deserialize<ProtobufResponseMessage>(stream);
                // Exception protobufResponse message
                case 2:
                    return Serializer.Deserialize<ProtobufExceptionResponseMessage>(stream);
                // Unused (3)- ignore
                default:
                    return null;
            }
        }

        /// <summary>
        /// Serializes message into byte array.
        /// </summary>
        /// <param name="msg">Message to serialize.</param>
        /// <returns>Serialized message.</returns>
        public byte[] Serialize(IMessage msg)
        {
            MemoryStream primaryStream = new MemoryStream();
            MemoryStream parameterStream = new MemoryStream();
            primaryStream.WriteByte(TypeToByte[msg.GetType()]);
            switch (msg.GetType().Name)
            {
                case nameof(ProtobufRequestMessage):
                    ProtobufRequestMessage protobufRequest = (ProtobufRequestMessage)msg;
                    if (protobufRequest.Args != null)
                    {
                        bool dropIp = InterfaceResolver.SenderEndPointIsExpectedByInterface(protobufRequest);
                        for (int index = 0;
                            index < protobufRequest.Args.Length && (!dropIp || index < protobufRequest.Args.Length - 1);
                            index++)
                        {
                            object obj = protobufRequest.Args[index];
                            Serializer.SerializeWithLengthPrefix(parameterStream, obj, PrefixStyle.Fixed32);
                        }
                    }
                    protobufRequest.SerializableArgs = parameterStream.ToArray();
                    Serializer.Serialize(primaryStream, msg);
                    break;
                case nameof(ProtobufResponseMessage):
                    ProtobufResponseMessage protobufResponse = (ProtobufResponseMessage) msg;
                    Serializer.Serialize(parameterStream, protobufResponse.Value);
                    protobufResponse.SerializableValue = parameterStream.ToArray();
                    Serializer.Serialize(primaryStream, msg);
                    break;
                default:
                    Serializer.Serialize(primaryStream, msg);
                    break;
            }
            return primaryStream.ToArray();
        }
    }
}