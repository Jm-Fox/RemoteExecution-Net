namespace RemoteExecution.Channels
{
	public interface IBroadcastChannel : IOutgoingMessageChannel
	{
		int ReceiverCount { get; }
	}
}