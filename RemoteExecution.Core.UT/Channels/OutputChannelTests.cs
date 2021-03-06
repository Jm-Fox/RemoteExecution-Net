using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Serializers;

namespace RemoteExecution.Core.UT.Channels
{
	public interface ITestableOutputChannel : IOutputChannel
	{
		byte[] SentData { get; }
	}

	public class TestableOutputChannel : OutputChannel, ITestableOutputChannel
	{
		private bool _isOpen = true;

		public TestableOutputChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

		#region ITestableOutputChannel Members

		public override bool IsOpen
		{
			get { return _isOpen; }
		}

		public byte[] SentData { get; private set; }

        #endregion

        public override void Close()
		{
			_isOpen = false;
			FireChannelClosed();
		}

        public override bool SendData(byte[] data)
		{
			SentData = data;
		    return true;
		}
	}

	[TestFixture]
	public class OutputChannelTests : ChannelTestsBase<TestableOutputChannel>
	{
		protected override TestableOutputChannel CreateSubject()
		{
			return new TestableOutputChannel(MessageSerializer);
		}
	}
}
