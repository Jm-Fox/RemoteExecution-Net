using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Output channel interface allowing to send messages.
	/// </summary>
	public interface IOutputChannel : IChannel
	{
		/// <summary>
		/// Sends given message through this channel.
		/// </summary>
		/// <param name="message">Message to send.</param>
		/// <exception cref="ConnectionOpenException">Thrown by non-durable connections when called if closed.</exception>
		/// <returns>True if the connection is open, false if the connection is paused.</returns>
		bool Send(IMessage message);
	}
}