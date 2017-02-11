using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.InterfaceResolution;

namespace RemoteExecution.Protobuf.UT.MS
{
    [TestClass]
    public class ProtobufSerializerTests
    {
        private readonly ProtobufMessageFactory factory = new ProtobufMessageFactory();
        private readonly ProtobufSerializer serializer = new ProtobufSerializer();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            InterfaceResolver.Singleton.RegisterInterface(typeof(ICalculator));
        }

        [TestMethod]
        public void ValidateRequestSerialization()
        {
            // Arrange
            IRequestMessage sent = factory.CreateRequestMessage("cat", "ICalculator", "Add", new object[] {5, 6}, true);

            // Act
            IRequestMessage received = (IRequestMessage)serializer.Deserialize(serializer.Serialize(sent));
            (received as IIncomplete).Complete(typeof(ICalculator).GetMethod(nameof(ICalculator.Add)));

            // Assert
            Assert.AreEqual(sent.IsResponseExpected, received.IsResponseExpected);
            CollectionAssert.AreEqual(sent.Args, received.Args);
            Assert.AreEqual(sent.MethodName, received.MethodName);
            Assert.AreEqual(sent.CorrelationId, received.CorrelationId);
            Assert.AreEqual(sent.MessageType, received.MessageType);
        }

        [TestMethod]
        public void ValidateResponseSerialization()
        {
            // Arrange
            IResponseMessage sent = factory.CreateResponseMessage("cat", 11);

            // Act
            IResponseMessage received = (IResponseMessage)serializer.Deserialize(serializer.Serialize(sent));
            (received as IIncomplete).Complete(typeof(ICalculator).GetMethod(nameof(ICalculator.Add)));

            // Assert
            Assert.AreEqual(sent.CorrelationId, received.CorrelationId);
            Assert.AreEqual(sent.MessageType, received.MessageType);
            Assert.AreEqual(sent.Value, received.Value);
        }

        [TestMethod]
        public void ValidateExceptionResponseSerialization()
        {
            // Arrange
            IExceptionResponseMessage sent = factory.CreateExceptionResponseMessage("cat", typeof(InvalidOperationException), "nope");

            // Act
            IExceptionResponseMessage received = (IExceptionResponseMessage)serializer.Deserialize(serializer.Serialize(sent));

            // Assert
            Assert.AreEqual(sent.CorrelationId, received.CorrelationId);
            Assert.AreEqual(sent.Message, received.Message);
            Assert.AreEqual(sent.ExceptionType, received.ExceptionType);
        }

        [TestMethod]
        public void ValidateIpEndPointIsDropped()
        {
            // Arrange
            IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);
            IRequestMessage sent = factory.CreateRequestMessage("cat", "ICalculator", "Divide", new object[] { 5, 6, null }, true);

            // Act
            IRequestMessage received = (IRequestMessage)serializer.Deserialize(serializer.Serialize(sent));
            received.Args = new object[] {any};
            (received as IIncomplete).Complete(typeof(ICalculator).GetMethod(nameof(ICalculator.Add)));

            // Assert
            Assert.AreEqual(sent.CorrelationId, received.CorrelationId);
            Assert.AreEqual(any, received.Args.Last());
        }
    }
}
