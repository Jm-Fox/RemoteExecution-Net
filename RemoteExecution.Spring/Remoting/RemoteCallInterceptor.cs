using AopAlliance.Intercept;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
	internal class RemoteCallInterceptor : IMethodInterceptor
	{
        private readonly RemoteExecutionPolicies _policies;
		private readonly IMethodInterceptor _oneWayInterceptor;
		private readonly IMethodInterceptor _twoWayInterceptor;

		public RemoteCallInterceptor(IMethodInterceptor oneWayInterceptor, IMethodInterceptor twoWayInterceptor, RemoteExecutionPolicies policies)
		{
			_oneWayInterceptor = oneWayInterceptor;
			_twoWayInterceptor = twoWayInterceptor;
		    _policies = policies;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
        {
            return _policies[invocation.Method].ActualReturnPolicy == ReturnPolicy.OneWay
                ? _oneWayInterceptor.Invoke(invocation)
                : _twoWayInterceptor.Invoke(invocation);
        }

		#endregion
	}
}