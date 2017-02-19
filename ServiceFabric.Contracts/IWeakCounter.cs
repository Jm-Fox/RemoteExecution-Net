using RemoteExecution.Remoting;

namespace ServiceFabric.Contracts
{
    [TimeoutPolicy(999)]
    public interface IWeakCounter
    {
        void Crash();

        void Increment();
        int GetCount();
    }
}