using RemoteExecution.Remoting;

namespace DurableServices.Contracts
{
    public interface ICalculator
    {
        [TimeoutPolicy(60 * 60)]
        int Add(int x, int y);
    }
}
