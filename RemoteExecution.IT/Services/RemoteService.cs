﻿using System;
using System.Threading;
using RemoteExecution.Connections;

namespace RemoteExecution.IT.Services
{
	public class RemoteService : IRemoteService
	{
		private readonly int _connectionId;
		private readonly IClientService _clientService;
		private readonly IBroadcastService _broadcastService;
		private readonly INetworkConnection _clientConnection;

		public RemoteService(int connectionId, IClientService clientService, IBroadcastService broadcastService, INetworkConnection clientConnection)
		{
			_connectionId = connectionId;
			_clientService = clientService;
			_broadcastService = broadcastService;
			_clientConnection = clientConnection;
		}

		public int GetConnectionId()
		{
			return _connectionId;
		}

		public int ExecuteChainedMethod()
		{
			return _clientService.GetClientValue() * 2;
		}

		public string Hello()
		{
			return "world";
		}

		public void ThrowException()
		{
			throw new MyException("test");
		}

		public void CloseConnectionOnServerSide()
		{
			_clientConnection.Dispose();
		}

		public void Sleep(TimeSpan timeSpan)
		{
			Thread.Sleep(timeSpan);
			_clientService.Callback(timeSpan);
		}

		public void Broadcast(int number)
		{
			_broadcastService.SetNumber(number);
		}
	}

	public class MyException : Exception
	{
		public MyException(string message)
			: base(message)
		{
		}
	}
}