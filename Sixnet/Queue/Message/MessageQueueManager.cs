using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Development.Message;
using Sixnet.Exceptions;
using static Sixnet.Cache.CacheManager;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Message queue manager
    /// </summary>
    public static class MessageQueueManager
    {
        #region Fields

        /// <summary>
        /// Message queue providers
        /// </summary>
        readonly static Dictionary<MessageQueueServerType, IMessageQueueProvider> QueueProviders = new();

        /// <summary>
        /// Default queue server
        /// </summary>
        static MessageQueueServer DefaultQueueServer;

        /// <summary>
        /// Get message queue server func
        /// </summary>
        static Func<SendMessageOptions, MessageQueueServer> GetMessageQueueServerFunc = DefaultGetMessageQueueServer; 

        #endregion

        #region Configuration

        /// <summary>
        /// Configure provider
        /// </summary>
        /// <param name="serverType">Service type</param>
        /// <param name="messageQueueProvider">Message queue provider</param>
        public static void ConfigureProvider(MessageQueueServerType serverType, IMessageQueueProvider messageQueueProvider)
        {
            ThrowHelper.ThrowArgNullIf(messageQueueProvider == null, nameof(messageQueueProvider));
            QueueProviders[serverType] = messageQueueProvider;
        }

        /// <summary>
        /// Configure message server
        /// </summary>
        /// <param name="getMessageServerFunc">Get message server func</param>
        public static void ConfigureMessageServer(Func<SendMessageOptions, MessageQueueServer> getMessageServerFunc)
        {
            if (getMessageServerFunc != null)
            {
                GetMessageQueueServerFunc = getMessageServerFunc;
            }
            else
            {
                GetMessageQueueServerFunc = DefaultGetMessageQueueServer;
            }
        }

        /// <summary>
        /// Configure default server
        /// </summary>
        /// <param name="messageQueueServer">Message queue server</param>
        public static void ConfigureDefaultServer(MessageQueueServer messageQueueServer)
        {
            ThrowHelper.ThrowArgNullIf(messageQueueServer == null, nameof(messageQueueServer));
            DefaultQueueServer = messageQueueServer;
        }

        #endregion

        #region Send message

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="sendMessageOptions">Send message options</param>
        /// <returns></returns>
        public static Task SendAsync(IEnumerable<SendMessageOptions> sendMessageOptions)
        {
            ThrowHelper.ThrowArgErrorIf(sendMessageOptions.IsNullOrEmpty(), "Message options is null or empty");

            var groupServerResult = GroupMessageQueueServer(sendMessageOptions);
            var serverOptionsGroup = groupServerResult.Item1;
            var servers = groupServerResult.Item2;
            var sendTasks = new List<Task>(servers.Count);

            foreach (var optionGroupItem in serverOptionsGroup)
            {
                var server = servers[optionGroupItem.Key];
                var serverProvider = QueueProviders[server.ServerType];
                foreach (var options in optionGroupItem.Value)
                {
                    sendTasks.Add(serverProvider.SendAsync(server, options));
                }
            }
            return Task.WhenAll(sendTasks);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="sendMessageOptions">Send message options</param>
        /// <returns>response</returns>
        public static void Send(IEnumerable<SendMessageOptions> sendMessageOptions)
        {
            ThrowHelper.ThrowArgErrorIf(sendMessageOptions.IsNullOrEmpty(), "Message options is null or empty");

            var groupServerResult = GroupMessageQueueServer(sendMessageOptions);
            var serverOptionsGroup = groupServerResult.Item1;
            var servers = groupServerResult.Item2;

            foreach (var optionGroupItem in serverOptionsGroup)
            {
                var server = servers[optionGroupItem.Key];
                var serverProvider = QueueProviders[server.ServerType];
                foreach (var options in optionGroupItem.Value)
                {
                    serverProvider.Send(server, options);
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
            ThrowHelper.ThrowArgNullIf(server == null, nameof(server));
            ThrowHelper.ThrowArgNullIf(listenOptions == null, nameof(listenOptions));
            ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(server.ServerType, out var provider), $"{server.ServerType} not set queue provider");
            return provider.ListenAsync(server, listenOptions);
        }

        /// <summary>
        /// Listen message queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="listenOptions">Listen options</param>
        public static void Listen(MessageQueueServer server, ListenMessageQueueOptions listenOptions)
        {
            ThrowHelper.ThrowArgNullIf(server == null, nameof(server));
            ThrowHelper.ThrowArgNullIf(listenOptions == null, nameof(listenOptions));
            ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(server.ServerType, out var provider), $"{server.ServerType} not set queue provider");
            provider.Listen(server, listenOptions);
        }

        #endregion

        #region Unlisten server

        /// <summary>
        /// Unlisten server
        /// </summary>
        /// <param name="servers">Servers</param>
        public static Task UnlistenServerAsync(params MessageQueueServer[] servers)
        {
            ThrowHelper.ThrowArgErrorIf(servers.IsNullOrEmpty(), "Message queue servers is null or empty");

            var serverTypes = servers.Select(c => c.ServerType).Distinct().ToList();
            var serverTasks = new List<Task>(serverTypes.Count);
            foreach (var serverType in serverTypes)
            {
                ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(serverType, out var provider), $"{serverType} not set queue provider");
                var currentServers = servers.Where(c => c.ServerType == serverType);
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
            ThrowHelper.ThrowArgErrorIf(servers.IsNullOrEmpty(), "Message queue servers is null or empty");

            var serverTypes = servers.Select(c => c.ServerType).Distinct().ToList();
            var serverTasks = new List<Task>(serverTypes.Count);
            foreach (var serverType in serverTypes)
            {
                ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(serverType, out var provider), $"{serverType} not set queue provider");
                var currentServers = servers.Where(c => c.ServerType == serverType);
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
            ThrowHelper.ThrowArgNullIf(server == null, nameof(server));
            ThrowHelper.ThrowArgErrorIf(queueNames.IsNullOrEmpty(), "Queue names is null or empty");
            ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(server.ServerType, out var provider), $"{server.ServerType} not set queue provider");
            return provider.UnlistenQueueAsync(server, queueNames);
        }

        /// <summary>
        /// Unlisten queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="queueNames">Queue names</param>
        public static void UnlistenQueue(MessageQueueServer server, params string[] queueNames)
        {
            ThrowHelper.ThrowArgNullIf(server == null, nameof(server));
            ThrowHelper.ThrowArgErrorIf(queueNames.IsNullOrEmpty(), "Queue names is null or empty");
            ThrowHelper.ThrowFrameworkErrorIf(!QueueProviders.TryGetValue(server.ServerType, out var provider), $"{server.ServerType} not set queue provider");
            provider.UnlistenQueue(server, queueNames);
        }

        #endregion

        #region Unlisten all queue

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        public static Task UnlistenAllAsync()
        {
            var queueTasks = new List<Task>(QueueProviders.Count);
            foreach (var providerItem in QueueProviders)
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
            var queueTasks = new List<Task>(QueueProviders.Count);
            foreach (var providerItem in QueueProviders)
            {
                providerItem.Value.UnlistenAll();
            }
        }

        #endregion

        #region Group message queue server

        static Tuple<Dictionary<string, List<SendMessageOptions>>, Dictionary<string, MessageQueueServer>> GroupMessageQueueServer(IEnumerable<SendMessageOptions> messageOptions)
        {
            var serverOptionsGroup = new Dictionary<string, List<SendMessageOptions>>();
            var servers = new Dictionary<string, MessageQueueServer>();
            foreach (var options in messageOptions)
            {
                if (options != null)
                {
                    var messageServer = GetServer(options);
                    if (messageServer != null)
                    {
                        if (servers.ContainsKey(messageServer.IdentityKey))
                        {
                            serverOptionsGroup[messageServer.IdentityKey].Add(options);
                        }
                        else
                        {
                            servers.Add(messageServer.IdentityKey, messageServer);
                            serverOptionsGroup.Add(messageServer.IdentityKey, new List<SendMessageOptions>()
                            {
                                options
                            });
                        }
                    }
                }
            }
            CheckQueueProvider(servers.Select(c => c.Value.ServerType).Distinct());
            return new Tuple<Dictionary<string, List<SendMessageOptions>>, Dictionary<string, MessageQueueServer>>(serverOptionsGroup, servers);
        }

        #endregion

        #region Get message server

        /// <summary>
        /// Get message queue server
        /// </summary>
        /// <param name="options">Message options</param>
        /// <returns></returns>
        static MessageQueueServer GetServer(SendMessageOptions options)
        {
            var server = GetMessageQueueServerFunc?.Invoke(options);
            ThrowHelper.ThrowFrameworkErrorIf(server == null, "Not config message queue server");
            return server;
        }

        static MessageQueueServer DefaultGetMessageQueueServer(SendMessageOptions options)
        {
            return DefaultQueueServer;
        }

        #endregion

        #region Check queue provider

        /// <summary>
        /// Check queue provider
        /// </summary>
        /// <param name="serverTypes">Message server types</param>
        static void CheckQueueProvider(IEnumerable<MessageQueueServerType> serverTypes)
        {
            if (serverTypes != null)
            {
                ThrowHelper.ThrowArgErrorIf(QueueProviders.IsNullOrEmpty(), "Not set message queue provider");
                foreach (var serverType in serverTypes)
                {
                    ThrowHelper.ThrowArgErrorIf(!QueueProviders.ContainsKey(serverType) || QueueProviders[serverType] == null, $"{serverType} not set queue provider");
                }
            }
        }

        #endregion
    }
}
