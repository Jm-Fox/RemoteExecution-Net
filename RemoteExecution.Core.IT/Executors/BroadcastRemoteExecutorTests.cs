using System;
using NUnit.Framework;
using RemoteExecution.Config;
using RemoteExecution.Core.IT.Helpers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;

namespace RemoteExecution.Core.IT.Executors
{
	[TestFixture]
	public class BroadcastRemoteExecutorTests
	{
		private BroadcastRemoteExecutor _subject;
		private MockBroadcastChannel _broadcastChannel;

		public interface ICalculator
		{
			int Add(int x, int y);
		}

		public interface IMyInterface : ICalculator
		{
			void VoidMethod(string text);
		}

		public interface INotifier
		{
			void Notify(string test);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_broadcastChannel = new MockBroadcastChannel();
			_subject = new BroadcastRemoteExecutor(_broadcastChannel, new DefaultMessageFactory());
		}

		#endregion

		[Test]
		public void Should_not_allow_to_generate_proxy_if_interface_have_two_way_methods()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Create<ICalculator>());
			Assert.That(ex.Message, Is.EqualTo("ICalculator interface cannot be used for broadcasting because some of its methods returns result."));
		}

		[Test]
		public void Should_not_allow_to_generate_proxy_if_interface_inherits_two_way_methods()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Create<IMyInterface>());
			Assert.That(ex.Message, Is.EqualTo("IMyInterface interface cannot be used for broadcasting because some of its methods returns result."));
		}

		[Test]
		public void Should_send_message_on_channel()
        {
            Configurator.Configure();
            bool wasCalled = false;
			_broadcastChannel.OnSend += m => wasCalled = true;
			_subject.Create<INotifier>().Notify("test");
			Assert.That(wasCalled, Is.True);
		}
	}
}