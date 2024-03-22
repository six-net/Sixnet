using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.MQ.InProcess;

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue manager
    /// </summary>
    public static class SixnetMQ
    {
        #region Fields

        static readonly InternalMessageQueueProvider _internalProvider = new();
        static readonly MessageQueueOptions _defaultMessageQueueOptions = new();
        static readonly MessageQueueServer _defaultInProcessServer = new() { Type = MessageQueueType.Internal };

        #endregion

        #region Enqueue

        /// <summary>
        /// Enqueue message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static Task EnqueueAsync(params SixnetQueueMessage[] messages)
        {
            IEnumerable<SixnetQueueMessage> messageCollection = messages;
            return EnqueueAsync(messageCollection);
        }

        /// <summary>
        /// Enqueue message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static Task EnqueueAsync(IEnumerable<SixnetQueueMessage> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var messageTasks = new List<Task>();
            foreach (var message in messages)
            {
                var endpoint = GetMessageQueueEndpoint(message);
                var provider = GetMessageQueueProvider(endpoint.Server.Type);

                messageTasks.Add(provider.EnqueueAsync(new EnqueueParameter()
                {
                    Endpoint = endpoint,
                    Message = message
                }));
            }
            return Task.WhenAll(messageTasks);
        }

        /// <summary>
        /// Enqueue message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static void Enqueue(params SixnetQueueMessage[] messages)
        {
            IEnumerable<SixnetQueueMessage> messageCollection = messages;
            Enqueue(messageCollection);
        }

        /// <summary>
        /// Enqueue message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static void Enqueue(IEnumerable<SixnetQueueMessage> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            foreach (var message in messages)
            {
                var endpoint = GetMessageQueueEndpoint(message);
                var provider = GetMessageQueueProvider(endpoint.Server.Type);

                provider.Enqueue(new EnqueueParameter()
                {
                    Endpoint = endpoint,
                    Message = message
                });
            }
        }

        #endregion

        #region Consume

        /// <summary>
        /// Consume message queue
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        public static Task ConsumeAsync(ConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            return provider.ConsumeAsync(parameter);
        }

        /// <summary>
        /// Consume message queue
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        public static void Consume(ConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            provider.Consume(parameter);
        }

        #endregion

        #region Abort Consume

        /// <summary>
        /// Abort consume message queue
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        public static Task AbortConsumeAsync(AbortConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            return provider.AbortConsumeAsync(parameter);
        }

        /// <summary>
        /// Abort consume message queue
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        public static void AbortConsume(AbortConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            provider.AbortConsume(parameter);
        }

        #endregion

        #region Add queue

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public static Task AddQueueAsync(AddQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            return provider.AddQueueAsync(parameter);
        }

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public static void AddQueue(AddQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            provider.AddQueue(parameter);
        }

        #endregion

        #region Delete queue

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public static Task DeleteQueueAsync(DeleteQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            return provider.DeleteQueueAsync(parameter);
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public static void DeleteQueue(DeleteQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.Server == null, nameof(ConsumeParameter.Server));

            var provider = GetMessageQueueProvider(parameter.Server.Type);
            provider.DeleteQueue(parameter);
        }

        #endregion

        #region Handle message

        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        internal static Task<bool> HandleMessageAsync(SixnetQueueMessage message)
        {
            if (message == null)
            {
                return Task.FromResult(false);
            }
            if (message is IInternalQueueMessage executableMessage)
            {
                return executableMessage.ExecuteAsync();
            }
            var options = GetMessageQueueOptions();
            var handler = options?.GetHandler(message.Topic);
            return handler?.Invoke(message) ?? Task.FromResult(false);
        }

        #endregion

        #region Get message queue endpoint

        /// <summary>
        /// Get message queue endpoint
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        static MessageQueueEndpoint GetMessageQueueEndpoint(SixnetQueueMessage message)
        {
            if (message is IInternalQueueMessage inProcessMessage)
            {
                return new MessageQueueEndpoint()
                {
                    Server = _defaultInProcessServer,
                    QueueNames = new List<string>() { inProcessMessage.QueueName }
                };
            }
            var endpoint = GetMessageQueueOptions()?.GetEndpoint(message);
            if(endpoint == null && message?.Group == QueueMessageGroupNames.DomainMessage)
            {
                return new MessageQueueEndpoint()
                {
                    Server = _defaultInProcessServer,
                    QueueNames = new List<string>() { InternalQueueNames.DomainMessage }
                };
            }

            SixnetDirectThrower.ThrowSixnetExceptionIf(endpoint?.Server == null, $"Not set message queue server for:{message.Group}-{message.Topic}");

            return endpoint;
        }

        #endregion

        #region Get message queue options

        /// <summary>
        /// Get message queue options
        /// </summary>
        /// <returns></returns>
        internal static MessageQueueOptions GetMessageQueueOptions()
        {
            return SixnetContainer.GetOptions<MessageQueueOptions>() ?? _defaultMessageQueueOptions;
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
            if (messageQueueType == MessageQueueType.Internal)
            {
                return _internalProvider;
            }
            var mqOptions = GetMessageQueueOptions();
            var provider = mqOptions?.GetProvider(messageQueueType);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider != null, $"Not set message queue provider for {messageQueueType}");

            return provider;
        }

        #endregion

        #region Internal queue names

        /// <summary>
        /// Internal queue names
        /// </summary>
        public static class InternalQueueNames
        {
            /// <summary>
            /// Data cache queue name
            /// </summary>
            public const string DataCache = "SIXNET_DATA_CACHE_QUEUE_NAME";
            /// <summary>
            /// Logging queue name
            /// </summary>
            public const string Logging = "SIXNET_LOGGING_QUEUE_NAME";
            /// <summary>
            /// DomainMessage queue name
            /// </summary>
            public const string DomainMessage = "SIXNET_DOMAIN_MESSAGE_QUEUE_NAME";
        }

        /// <summary>
        /// Queue message group names
        /// </summary>
        public static class QueueMessageGroupNames
        {
            /// <summary>
            /// Domain message group
            /// </summary>
            public const string DomainMessage = "SIXNET_DOMAIN_MESSAGE_GROUP";
        }

        #endregion
    }
}
