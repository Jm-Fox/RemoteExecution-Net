﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Lidgren.Network;

namespace RemoteExecution
{
    /// <summary>
    /// Provides a loop for reading messages from a NetPeer.
    /// </summary>
	public class MessageLoop
	{
		private readonly Action<NetIncomingMessage> _handleMessage;
		private readonly NetPeer _peer;
		private readonly SemaphoreSlim _semaphore;
		private readonly Thread _thread;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public MessageLoop(NetPeer peer, Action<NetIncomingMessage> handleMessage)
		{
			_peer = peer;
			_handleMessage = handleMessage;
			_thread = new Thread(Run) { Name = "Message loop", IsBackground = true };

			_semaphore = new SemaphoreSlim(0);
			_thread.Start();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void SetSynchronizationContext()
		{
			if (SynchronizationContext.Current != null)
				return;
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

        /// <summary>
        /// Disposes the message loop.
        /// </summary>
        public void Dispose()
		{
			_semaphore.Release();
			_thread.Join();
			_semaphore.Dispose();
		}

		private void MessageReady(object obj)
		{
			var msg = _peer.ReadMessage();
			if (msg != null)
				_handleMessage(msg);
		}

		private void Run()
		{
			SetSynchronizationContext();
			_peer.RegisterReceivedCallback(MessageReady);
			_semaphore.Wait();
			_peer.UnregisterReceivedCallback(MessageReady);
		}
	}
}