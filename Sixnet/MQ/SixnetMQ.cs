using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue manager
    /// </summary>
    public static class SixnetMQ
    {
        #region Fields

        /// <summary>
        /// Message queue providers
        /// </summary>
        readonly static Dictionary<MessageQueueServerType, ISixnetMessageQueueProvider> _serverQueueProviders = new();

        /// <summary>
        /// Default queue server
        /// </summary>
        static MessageQueueServer _defaultQueueServer;

        /// <summary>
        /// Get message queue server func
        /// </summary>
        static Func<ServerQueueMessage, MessageQueueServer> _getMessageQueueServerFunc = GetDefaultMessageQueueServer;

        #endregion

        #region Configuration

        /// <summary>
        /// Configure provider
        /// </summary>
        /// <param name="serverType">Service type</param>
        /// <param name="messageQueueProvider">Message queue provider</param>
        public static void ConfigureProvider(MessageQueueServerType serverType, ISixnetMessageQueueProvider messageQueueProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(messageQueueProvider == null, nameof(messageQueueProvider));
            _serverQueueProviders[serverType] = messageQueueProvider;
        }

        /// <summary>
        /// Configure message server
        /// </summary>
        /// <param name="getMessageServerFunc">Get message server func</param>
        public static void ConfigureServer(Func<ServerQueueMessage, MessageQueueServer> getMessageServerFunc)
        {
            if (getMessageServerFunc != null)
            {
                _getMessageQueueServerFunc = getMessageServerFunc;
            }
            else
            {
                _getMessageQueueServerFunc = GetDefaultMessageQueueServer;
            }
        }

        /// <summary>
        /// Configure default server
        /// </summary>
        /// <param name="messageQueueServer">Message queue server</param>
        public static void ConfigureServer(MessageQueueServer messageQueueServer)
        {
            SixnetDirectThrower.ThrowArgNullIf(messageQueueServer == null, nameof(messageQueueServer));
            _defaultQueueServer = messageQueueServer;
        }

        #endregion

        #region Send message

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Send message options</param>
        /// <returns></returns>
        public static Task SendAsync(params ISixnetQueueMessage[] messages)
        {
            IEnumerable<ISixnetQueueMessage> messageCollection = messages;
            return SendAsync(messageCollection);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Send message options</param>
        /// <returns></returns>
        public static Task SendAsync(IEnumerable<ISixnetQueueMessage> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var serverMessageTasks = new List<Task>();
            foreach (var message in messages)
            {
                if (message is InProcessQueueMessage inProcessQueueMessage)
                {
                    var queue = InProcessQueueManager.GetQueue(inProcessQueueMessage.QueueName);
                    queue.Enqueue(inProcessQueueMessage);
                }
                if (message is ServerQueueMessage serverQueueMessage)
                {
                    var messageServer = GetMessageQueueServer(serverQueueMessage);
                    if (messageServer != null)
                    {
                        SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(messageServer.Type, out var provider) || provider == null
                        , $"Not config provider for {messageServer.Type}");
                        serverMessageTasks.Add(provider.SendAsync(messageServer, serverQueueMessage));
                    }
                }
            }
            return Task.WhenAll(serverMessageTasks);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns>response</returns>
        public static void Send(params ISixnetQueueMessage[] messages)
        {
            IEnumerable<ISixnetQueueMessage> messageCollection = messages;
            Send(messageCollection);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns>response</returns>
        public static void Send(IEnumerable<ISixnetQueueMessage> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            foreach (var message in messages)
            {
                if (message is InProcessQueueMessage inProcessQueueMessage)
                {
                    var queue = InProcessQueueManager.GetQueue(inProcessQueueMessage.QueueName);
                    queue.Enqueue(inProcessQueueMessage);
                }
                if (message is ServerQueueMessage serverQueueMessage)
                {
                    var messageServer = GetMessageQueueServer(serverQueueMessage);
                    if (messageServer != null)
                    {
                        SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(messageServer.Type, out var provider) || provider == null
                        , $"Not config provider for {messageServer.Type}");
                        provider.Send(messageServer, serverQueueMessage);
                    }
                }
            }
        }

        #endregion

        #region Listen

        /// <summary>
        /// Listen message queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="listenOptions">Listen options</param>
        public static Task ListenAsync(MessageQueueServer server, ListenMessageQueueOptions listenOptions)
        {
            SixnetDirectThrower.ThrowArgNullIf(server == null, nameof(server));
            SixnetDirectThrower.ThrowArgNullIf(listenOptions == null, nameof(listenOptions));
            SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(server.Type, out var provider) || provider == null, $"{server.Type} not set queue provider");
            return provider.ListenAsync(server, listenOptions);
        }

        /// <summary>
        /// Listen message queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="listenOptions">Listen options</param>
        public static void Listen(MessageQueueServer server, ListenMessageQueueOptions listenOptions)
        {
            SixnetDirectThrower.ThrowArgNullIf(server == null, nameof(server));
            SixnetDirectThrower.ThrowArgNullIf(listenOptions == null, nameof(listenOptions));
            SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(server.Type, out var provider) || provider == null, $"{server.Type} not set queue provider");
            provider.Listen(server, listenOptions);
        }

        #endregion

        #region Unlisten server

        /// <summary>
        /// Unlisten server
        /// </summary>
        /// <param name="servers">Servers</param>
        public static Task UnlistenServerAsync(IEnumerable<MessageQueueServer> servers)
        {
            SixnetDirectThrower.ThrowArgErrorIf(servers.IsNullOrEmpty(), "Message queue servers is null or empty");

            var serverTypes = servers.Select(c => c.Type).Distinct().ToList();
            var serverTasks = new List<Task>(serverTypes.Count);
            foreach (var serverType in serverTypes)
            {
                SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(serverType, out var provider), $"{serverType} not set queue provider");
                var currentServers = servers.Where(c => c.Type == serverType);
                serverTasks.Add(provider.UnlistenServerAsync(currentServers.ToArray()));
            }
            return Task.WhenAll(serverTasks);
        }

        /// <summary>
        /// Unlisten server
        /// </summary>
        /// <param name="servers">Servers</param>
        public static void UnlistenServer(params MessageQueueServer[] servers)
        {
            SixnetDirectThrower.ThrowArgErrorIf(servers.IsNullOrEmpty(), "Message queue servers is null or empty");

            var serverTypes = servers.Select(c => c.Type).Distinct().ToList();
            var serverTasks = new List<Task>(serverTypes.Count);
            foreach (var serverType in serverTypes)
            {
                SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(serverType, out var provider), $"{serverType} not set queue provider");
                var currentServers = servers.Where(c => c.Type == serverType);
                provider.UnlistenServer(currentServers.ToArray());
            }
        }

        #endregion

        #region Unlisten queue

        /// <summary>
        /// Unlisten queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="queueNames">Queue names</param>
        public static Task UnlistenQueueAsync(MessageQueueServer server, params string[] queueNames)
        {
            SixnetDirectThrower.ThrowArgNullIf(server == null, nameof(server));
            SixnetDirectThrower.ThrowArgErrorIf(queueNames.IsNullOrEmpty(), "Queue names is null or empty");
            SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(server.Type, out var provider), $"{server.Type} not set queue provider");
            return provider.UnlistenQueueAsync(server, queueNames);
        }

        /// <summary>
        /// Unlisten queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="queueNames">Queue names</param>
        public static void UnlistenQueue(MessageQueueServer server, params string[] queueNames)
        {
            SixnetDirectThrower.ThrowArgNullIf(server == null, nameof(server));
            SixnetDirectThrower.ThrowArgErrorIf(queueNames.IsNullOrEmpty(), "Queue names is null or empty");
            SixnetDirectThrower.ThrowSixnetExceptionIf(!_serverQueueProviders.TryGetValue(server.Type, out var provider), $"{server.Type} not set queue provider");
            provider.UnlistenQueue(server, queueNames);
        }

        #endregion

        #region Unlisten all queue

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        public static Task UnlistenAllAsync()
        {
            var queueTasks = new List<Task>(_serverQueueProviders.Count);
            foreach (var providerItem in _serverQueueProviders)
            {
                queueTasks.Add(providerItem.Value.UnlistenAllAsync());
            }
            return Task.WhenAll(queueTasks);
        }

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        public static void UnlistenAll()
        {
            var queueTasks = new List<Task>(_serverQueueProviders.Count);
            foreach (var providerItem in _serverQueueProviders)
            {
                providerItem.Value.UnlistenAll();
            }
        }

        #endregion

        #region Get message server

        /// <summary>
        /// Get message queue server
        /// </summary>
        /// <param name="options">Message options</param>
        /// <returns></returns>
        static MessageQueueServer GetMessageQueueServer(ServerQueueMessage options)
        {
            var server = _getMessageQueueServerFunc?.Invoke(options);
            SixnetDirectThrower.ThrowSixnetExceptionIf(server == null, "Not config message queue server");
            return server;
        }

        /// <summary>
        /// Get default message queue server
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        static MessageQueueServer GetDefaultMessageQueueServer(ServerQueueMessage options)
        {
            return _defaultQueueServer;
        }

        #endregion

        #region InProcessQueueNames

        /// <summary>
        /// In process queue names
        /// </summary>
        public static class InProcessQueueNames
        {
            /// <summary>
            /// Data cache queue name
            /// </summary>
            public const string DataCache = "SIXNET_DATA_CACHE";
            /// <summary>
            /// Logging queue name
            /// </summary>
            public const string Logging = "SIXNET_LOGGING";
            /// <summary>
            /// Message queue name
            /// </summary>
            public const string Message = "SIXNET_MESSAGE";
        }

        #endregion
    }
}
