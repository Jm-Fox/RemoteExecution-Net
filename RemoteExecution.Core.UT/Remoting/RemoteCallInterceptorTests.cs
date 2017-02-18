using AopAlliance.Intercept;
using NUnit.Framework;
using RemoteExecution.Executors;
using RemoteExecution.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.UT.Remoting
{
	[TestFixture]
	public class RemoteCallInterceptorTests
	{
		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
            [ForcedReturnPolicy(ReturnPolicy.OneWay)]
            void DontNotifyAgain(string text);
            [ForcedReturnPolicy(ReturnPolicy.TwoWay)]
            void NotifyMeAgain(string text);
            [ForcedReturnPolicy(ReturnPolicy.OneWay)]
		    string IgnoreHello(int x);
		}

		private IMethodInterceptor _oneWayInterceptor;
		private IMethodInterceptor _twoWayInterceptor;

		private ITestInterface GetInvocationHelper(ReturnPolicy returnPolicy = ReturnPolicy.TwoWay)
		{
            RemoteExecutionPolicies policies = new RemoteExecutionPolicies(typeof(ITestInterface), returnPolicy);
			var subject = new RemoteCallInterceptor(_oneWayInterceptor, _twoWayInterceptor, policies);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_oneWayInterceptor = MockRepository.GenerateMock<IMethodInterceptor>();
			_twoWayInterceptor = MockRepository.GenerateMock<IMethodInterceptor>();
		}

		#endregion

		[Test]
		[TestCase(ReturnPolicy.OneWay)]
		[TestCase(ReturnPolicy.TwoWay)]
		public void Should_always_execute_operations_returning_result_as_two_way(ReturnPolicy executionMode)
		{
			GetInvocationHelper(executionMode).Hello(5);
			_twoWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_oneWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}

        [Test]
        [TestCase(ReturnPolicy.OneWay)]
        [TestCase(ReturnPolicy.TwoWay)]
        public void Should_defer_execute_operations_returning_result_to_attribute_two_way(ReturnPolicy executionMode)
        {
            GetInvocationHelper(executionMode).NotifyMeAgain("test");
            _twoWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
            _oneWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
        }

        [Test]
        [TestCase(ReturnPolicy.OneWay)]
        [TestCase(ReturnPolicy.TwoWay)]
        public void Should_defer_execute_operations_returning_result_to_attribute_one_way_1(ReturnPolicy executionMode)
        {
            GetInvocationHelper(executionMode).DontNotifyAgain("test");
            _oneWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
            _twoWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
        }

        [Test]
        [TestCase(ReturnPolicy.OneWay)]
        [TestCase(ReturnPolicy.TwoWay)]
        public void Should_defer_execute_operations_returning_result_to_attribute_one_way_2(ReturnPolicy executionMode)
        {
            GetInvocationHelper(executionMode).IgnoreHello(5);
            _oneWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
            _twoWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
        }

        [Test]
		public void Should_execute_no_result_returning_operation_as_one_way()
		{
			GetInvocationHelper(ReturnPolicy.OneWay).Notify("test");
			_oneWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_twoWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}

		[Test]
		public void Should_wait_for_response_if_one_way_method_is_called_in_sync_mode()
		{
			GetInvocationHelper().Notify("test");
			_twoWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_oneWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}
	}
}
