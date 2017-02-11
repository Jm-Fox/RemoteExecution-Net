using System.Reflection;
using ObjectSerialization;
using ObjectSerialization.Types;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.InterfaceResolution;
using RemoteExecution.TransportLayer;

namespace RemoteExecution.Serializers
{
	/// <summary>
	/// Message serializer class using ObjectSerialization framework to serialize/deserialize messages.
	/// </summary>
	public class BinaryMessageSerializer : IMessageSerializer
	{
		private static readonly IObjectSerializer _serializer = new ObjectSerializer();

		static BinaryMessageSerializer()
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(typeof(TransportLayerResolver).Assembly);
		}

		#region IMessageSerializer Members

		/// <summary>
		/// Deserializes byte array into message object.
		/// </summary>
		/// <param name="msg">Serialized message.</param>
		/// <returns>Deserialized message.</returns>
		public IMessage Deserialize(byte[] msg)
		{
			return _serializer.Deserialize<IMessage>(msg);
		}

		/// <summary>
		/// Serializes message into byte array.
		/// </summary>
		/// <param name="msg">Message to serialize.</param>
		/// <returns>Serialized message.</returns>
		public byte[] Serialize(IMessage msg)
		{
            IRequestMessage request = msg as IRequestMessage;
		    if (request != null && InterfaceResolver.SenderEndPointIsExpectedByInterface(request))
		    {
                object[] args2 = new object[request.Args.Length - 1];
		        for (int i = 0; i < args2.Length; i++)
		            args2[i] = request.Args[i];
		        request.Args = args2;
		    }
		    return _serializer.Serialize(msg);
		}

		#endregion

		/// <summary>
		/// Registers all classes with [Serializable] attribute in order to decrease size of serialized message containing them.
		/// </summary>
		/// <param name="assembly"></param>
		public static void RegsterSerializableFrom(Assembly assembly)
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(assembly);
		}
	}
}