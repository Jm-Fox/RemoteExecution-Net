using RemoteExecution.Remoting;
using RemoteExecution.Executors;

namespace ServiceFabric.Contracts
{
    [TimeoutPolicy(999)]
    public interface IWeakCalculator
    {
        [ForcedReturnPolicy(ReturnPolicy.OneWay)]
        void Crash();
        
        int Add(int x, int y);
    }
}
