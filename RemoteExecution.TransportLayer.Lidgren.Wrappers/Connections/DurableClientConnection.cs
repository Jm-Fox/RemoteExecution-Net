﻿using System;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Connections
{
    /// <summary>
    /// Same as ClientConnection, except exposes the durable channel's ConnectionClose event.
    /// Can only be used with DurableLidgrenClientChannel
    /// </summary>
    public class DurableClientConnection : ClientConnection
    {
        /// <summary>
        /// [Singular] event fired when the connection is closed. The listener can suggest a new host and port where the service might be located.
        /// If not used, reconnection attempts will hit the same host and port.
        /// </summary>
        public Action<ClosedConnectionResponse> HandleClosedConnectionResponse
        {
            get { return ((DurableLidgrenClientChannel) Channel).HandleClosedConnectionResponse; }
            set { ((DurableLidgrenClientChannel) Channel).HandleClosedConnectionResponse = value; }
        }

        /// <summary>
        /// How many times the channel can try to reconnect before aborting.
        /// </summary>
        public int RetryAttempts
        {
            get { return ((DurableLidgrenClientChannel) Channel).RetryAttempts; }
            set { ((DurableLidgrenClientChannel) Channel).RetryAttempts = value; }
        }

        #region ClientConnection Members

                /// <summary>
                /// Creates client connection instance with channel constructed from connectionUri, default operation dispatcher and connection configuration (<see cref="DefaultConfig"/>).
                /// </summary>
                /// <param name="connectionUri">Connection uri used to create channel.</param>
                public DurableClientConnection(string connectionUri)
            : base(connectionUri, new OperationDispatcher(), new ConnectionConfig())
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given operation dispatcher and default connection configuration (<see cref="DefaultConfig"/>).
        /// </summary>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
        public DurableClientConnection(string connectionUri, IOperationDispatcher dispatcher)
            : base(connectionUri, dispatcher, new ConnectionConfig())
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given connection configuration and default operation dispatcher.
        /// </summary>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="config">Connection configuration.</param>
        public DurableClientConnection(string connectionUri, IConnectionConfig config)
            : base(connectionUri, new OperationDispatcher(), config)
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given connection configuration and operation dispatcher.
        /// </summary>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
        /// <param name="config">Connection configuration.</param>
        public DurableClientConnection(string connectionUri, IOperationDispatcher dispatcher, IConnectionConfig config)
            : base(connectionUri, dispatcher, config)
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given operation dispatcher and default connection configuration (<see cref="DefaultConfig"/>).
        /// </summary>
        /// <param name="clientId">Identifier used to share client channels.</param>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
        public DurableClientConnection(string connectionUri, string clientId, IOperationDispatcher dispatcher)
            : base(clientId, connectionUri, dispatcher, new ConnectionConfig())
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given connection configuration and default operation dispatcher.
        /// </summary>
        /// <param name="clientId">Identifier used to share client channels.</param>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="config">Connection configuration.</param>
        public DurableClientConnection(string connectionUri, string clientId, IConnectionConfig config)
            : base(clientId, connectionUri, new OperationDispatcher(), config)
        {
        }

        /// <summary>
        /// Creates client connection instance with channel constructed from connectionUri, given connection configuration and operation dispatcher.
        /// </summary>
        /// <param name="clientId">Identifier used to share client channels.</param>
        /// <param name="connectionUri">Connection uri used to create channel.</param>
        /// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
        /// <param name="config">Connection configuration.</param>
        public DurableClientConnection(string connectionUri, string clientId, IOperationDispatcher dispatcher,
            IConnectionConfig config)
            : base(clientId, connectionUri, dispatcher, config)
        {
        }

        /// <summary>
        /// Creates client connection instance with given channel, connection configuration and operation dispatcher.
        /// </summary>
        /// <param name="channel">Communication channel used by connection.</param>
        /// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
        /// <param name="config">Connection configuration.</param>
        public DurableClientConnection(IClientChannel channel, IOperationDispatcher dispatcher, IConnectionConfig config)
            : base(channel, dispatcher, config)
        {
        }

        #endregion
    }
}