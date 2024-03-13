using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sixnet.Cache.SixnetCacher;

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue manager
    /// </summary>
    public static class SixnetMQ
    {
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
                        var provider = GetMessageQueueProvider(messageServer.Type);

                        SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set provider for {messageServer.Type}");

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
                        var provider = GetMessageQueueProvider(messageServer.Type);
                        SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set provider for {messageServer.Type}");
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

            var provider = GetMessageQueueProvider(server.Type);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{server.Type} not set queue provider");

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

            var provider = GetMessageQueueProvider(server.Type);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{server.Type} not set queue provider");

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
            foreach (var databaseType in serverTypes)
            {
                var provider = GetMessageQueueProvider(databaseType);

                SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{databaseType} not set queue provider");

                var currentServers = servers.Where(c => c.Type == databaseType);
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
            foreach (var databaseType in serverTypes)
            {
                var provider = GetMessageQueueProvider(databaseType);

                SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{databaseType} not set queue provider");

                var currentServers = servers.Where(c => c.Type == databaseType);
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

            var provider = GetMessageQueueProvider(server.Type);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{server.Type} not set queue provider");

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

            var provider = GetMessageQueueProvider(server.Type);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"{server.Type} not set queue provider");

            provider.UnlistenQueue(server, queueNames);
        }

        #endregion

        #region Unlisten all queue

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        public static Task UnlistenAllAsync()
        {
            var providers = GetMessageQueueOptions()?.GetAllProviders();
            if (!providers.IsNullOrEmpty())
            {
                var queueTasks = new List<Task>();
                foreach (var provider in providers)
                {
                    queueTasks.Add(provider.UnlistenAllAsync());
                }
                return Task.WhenAll(queueTasks);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        public static void UnlistenAll()
        {
            var providers = GetMessageQueueOptions()?.GetAllProviders();
            if (!providers.IsNullOrEmpty())
            {
                foreach (var provider in providers)
                {
                    provider.UnlistenAll();
                }
            }
        }

        #endregion

        #region Get message server

        /// <summary>
        /// Get message queue server
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        static MessageQueueServer GetMessageQueueServer(ServerQueueMessage message)
        {
            var server = GetMessageQueueOptions()?.GetServer(message);

            SixnetDirectThrower.ThrowSixnetExceptionIf(server == null, "Not set message queue server");

            return server;
        }

        #endregion

        #region Get message queue options

        /// <summary>
        /// Get message queue options
        /// </summary>
        /// <returns></returns>
        internal static MessageQueueOptions GetMessageQueueOptions()
        {
            return SixnetContainer.GetOptions<MessageQueueOptions>();
        }

        #endregion

        #region Get message queue provider

        /// <summary>
        /// Get message queue provider
        /// </summary>
        /// <param name="messageQueueType">Message queue type</param>
        /// <returns></returns>
        internal static ISixnetMessageQueueProvider GetMessageQueueProvider(MessageQueueType messageQueueType)
        {
            var mqOptions = GetMessageQueueOptions();
            return mqOptions?.GetProvider(messageQueueType);
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
