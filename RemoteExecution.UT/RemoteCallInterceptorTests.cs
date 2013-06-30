﻿using System.Linq;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;
using RemoteExecution.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class RemoteCallInterceptorTests
	{
		class TestableRemoteCallInterceptor : RemoteCallInterceptor
		{
			private readonly IResponseHandler _responseHandler;

			public TestableRemoteCallInterceptor(IOperationDispatcher operationDispatcher, IMessageChannel channel, IResponseHandler responseHandler, string interfaceName, OneWayMethodExcecution oneWayMethodExcecution)
				: base(operationDispatcher, channel, interfaceName, oneWayMethodExcecution)
			{
				_responseHandler = responseHandler;
			}

			protected override IResponseHandler CreateResponseHandler()
			{
				return _responseHandler;
			}
		}

		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
		}

		private IOperationDispatcher _dispatcher;
		private IMessageChannel _channel;
		private IResponseHandler _responseHandler;
		private MockRepository _repository;
		private const string _handlerId = "handlerID";
		private const string _interfaceName = "testInterface";

		[SetUp]
		public void SetUp()
		{
			_repository = new MockRepository();
			_dispatcher = _repository.DynamicMock<IOperationDispatcher>();
			_channel = _repository.DynamicMock<IMessageChannel>();
			_responseHandler = _repository.DynamicMock<IResponseHandler>();
		}

		private ITestInterface GetInvocationHelper(OneWayMethodExcecution oneWayMethodExcecution = OneWayMethodExcecution.Synchronized)
		{
			var subject = new TestableRemoteCallInterceptor(_dispatcher, _channel, _responseHandler, _interfaceName, oneWayMethodExcecution);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		[Test]
		[TestCase(OneWayMethodExcecution.Asynchronized)]
		[TestCase(OneWayMethodExcecution.Synchronized)]
		public void Should_always_execute_two_way_operations_synchronously(OneWayMethodExcecution executionMode)
		{
			using (_repository.Ordered())
			{
				Expect.Call(() => _dispatcher.RegisterResponseHandler(_responseHandler));
				Expect.Call(() => _channel.Send(Arg<IMessage>.Is.Anything));
				Expect.Call(() => _responseHandler.WaitForResponse());
				Expect.Call(() => _dispatcher.UnregisterResponseHandler(_responseHandler));
				Expect.Call(() => _responseHandler.GetValue());
			}
			_repository.ReplayAll();
			GetInvocationHelper(executionMode).Hello(5);
			_repository.VerifyAll();
		}

		[Test]
		public void Should_not_wait_for_response_if_one_way_method_is_called_in_async_mode()
		{
			_repository.ReplayAll();
			GetInvocationHelper(OneWayMethodExcecution.Asynchronized).Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
			_dispatcher.AssertWasNotCalled(d => d.RegisterResponseHandler(_responseHandler));
			_responseHandler.AssertWasNotCalled(h => h.WaitForResponse());
		}

		[Test]
		public void Should_wait_for_response_if_one_way_method_is_called_in_sync_mode()
		{
			_repository.ReplayAll();
			GetInvocationHelper().Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
			_dispatcher.AssertWasCalled(d => d.RegisterResponseHandler(_responseHandler));
			_responseHandler.AssertWasCalled(h => h.WaitForResponse());
		}

		[Test]
		public void Should_send_message_with_handler_id()
		{
			Expect.Call(_responseHandler.Id).Return(_handlerId);
			_repository.ReplayAll();
			GetInvocationHelper().Hello(5);
			_channel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Matches(m => m.CorrelationId == _handlerId)));
		}

		[Test]
		public void Should_send_message_with_method_details()
		{
			Expect.Call(_responseHandler.Id).Return(_handlerId);
			_repository.ReplayAll();

			const int methodArg = 5;
			GetInvocationHelper().Hello(methodArg);

			_channel.AssertWasCalled(ch => ch.Send(Arg<Request>.Matches(m =>
				m.Args.SequenceEqual(new object[] { methodArg }) &&
				m.OperationName == "Hello" &&
				m.GroupId == _interfaceName)));
		}
	}
}
