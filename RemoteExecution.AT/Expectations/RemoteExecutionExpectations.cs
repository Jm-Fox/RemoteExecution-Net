﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.AT.Expectations
{
	public abstract partial class BehaviorExpectations
	{
		[Test]
		public void Should_successfuly_execute_remote_operation()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var calculator = client.Executor.Create<ICalculator>();
				var greeter = client.Executor.Create<IGreeter>();

				Assert.That(calculator.Add(3, 5), Is.EqualTo(8));
				Assert.That(greeter.Hello("Josh"), Is.EqualTo("Hello Josh!"));
			}
		}

		[Test]
		public void Should_pass_server_side_exceptions()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var calculator = client.Executor.Create<ICalculator>();

				Assert.Throws<DivideByZeroException>(() => calculator.Divide(4, 0));
			}
		}

		[Test]
		public void Should_call_one_way_method_asynchronously()
		{
			var sleepTime = TimeSpan.FromMilliseconds(250);

			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var remote = client.Executor.Create<IRemoteService>(NoResultMethodExecution.OneWay);

				var watch = new Stopwatch();
				watch.Start();
				remote.Sleep(sleepTime);
				watch.Stop();

				Assert.That(watch.Elapsed, Is.LessThan(sleepTime));
			}
		}

		[Test]
		public void Should_call_one_way_method_synchronously_by_default()
		{
			var sleepTime = TimeSpan.FromMilliseconds(250);

			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var remote = client.Executor.Create<IRemoteService>();

				var watch = new Stopwatch();
				watch.Start();
				remote.Sleep(sleepTime);
				watch.Stop();

				Assert.That(watch.Elapsed, Is.GreaterThanOrEqualTo(sleepTime));
			}
		}

		[Test]
		public void Should_throw_if_connection_is_closed_during_remote_operation_call()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var remote = client.Executor.Create<IRemoteService>();

				var ex = Assert.Throws<OperationAbortedException>(remote.CloseConnectionOnServerSide);
				Assert.That(ex.Message, Is.EqualTo("Connection has been closed."));
			}
		}
	}
}
