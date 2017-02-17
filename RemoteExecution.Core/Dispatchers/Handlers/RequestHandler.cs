using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.InterfaceResolution;

namespace RemoteExecution.Dispatchers.Handlers
{
	internal class RequestHandler : IMessageHandler
	{
	    private IMessageFactory _messageFactory;
		public object Handler { get; private set; }
		public Type InterfaceType { get; private set; }

        public RequestHandler(Type interfaceType, object handler)
            : this(interfaceType, handler, new DefaultMessageFactory())
        {
        }

        public RequestHandler(Type interfaceType, object handler, IMessageFactory messageFactory)
		{
			InterfaceType = interfaceType;
			Handler = handler;
			HandledMessageType = interfaceType.Name;
			HandlerGroupId = interfaceType.GUID;
		    _messageFactory = messageFactory;
		}

		#region IMessageHandler Members

		public string HandledMessageType { get; private set; }
		public Guid HandlerGroupId { get; private set; }
		public void Handle(IMessage message)
		{
			var request = (IRequestMessage)message;
		    var methodInfo = InterfaceResolver.Singleton.GetInterface(request.MessageType)?.GetMethod(request.MethodName);
            if (methodInfo != null)
                (request as IIncomplete)?.Complete(methodInfo);

            if (request.IsResponseExpected)
				ExecuteWithResponse(request);
			else
				ExecuteWithoutResponse(request);
		}

		#endregion

		private object Execute(IRequestMessage requestMessage)
		{
			try
			{
				return InterfaceType.InvokeMember(requestMessage.MethodName, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, Handler, requestMessage.Args);
			}
			catch (MissingMemberException)
			{
				throw new InvalidOperationException(string.Format(
					"Unable to call {0}({1}) method on {2} handler: no matching method was found.",
					requestMessage.MethodName,
					string.Join(",", requestMessage.Args.Select(a => a == null ? "null" : a.GetType().Name)),
					HandledMessageType));
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		private void ExecuteWithResponse(IRequestMessage msg)
		{
			try
			{
				msg.Channel.Send(_messageFactory.CreateResponseMessage(msg.CorrelationId, Execute(msg)));
			}
			catch (Exception e)
			{
				msg.Channel.Send(_messageFactory.CreateExceptionResponseMessage(msg.CorrelationId, e.GetType(), e.Message));
			}
		}

		private void ExecuteWithoutResponse(IRequestMessage msg)
		{
			try
			{
				Execute(msg);
			}
			catch (Exception)
			{

			}
		}
	}
}