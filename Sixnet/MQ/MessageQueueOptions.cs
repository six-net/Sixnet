using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue options
    /// </summary>
    public class MessageQueueOptions
    {
        #region Fields

        /// <summary>
        /// Message queue providers
        /// </summary>
        readonly Dictionary<MessageQueueType, ISixnetMessageQueueProvider> _providers = new();
        readonly Dictionary<string, Func<SixnetQueueMessage, Task<bool>>> _messageHandlers = new();

        #endregion

        #region Properties

        /// <summary>
        /// Get message queue endpoint
        /// </summary>
        public Func<SixnetQueueMessage, MessageQueueEndpoint> GetMessageQueueEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the default message queue endpoint
        /// </summary>
        public MessageQueueEndpoint Endpoint { get; set; }

        /// <summary>
        /// Whether auto consome internal queue.
        /// Default is true.
        /// </summary>
        public bool AutoConsumeInternalQueue { get; set; } = true;

        /// <summary>
        /// Whether create internal queue
        /// </summary>
        public bool AutoCreateInternalQueue { get; set; } = true;

        /// <summary>
        /// Whether enable default domain message internal queue
        /// </summary>
        public bool EnableDefaultDomainMessageInternalQueue { get; set; } = true;

        /// <summary>
        /// Gets or sets the mandatory callback
        /// </summary>
        public Func<SixnetQueueMessage, bool> MandatoryCallback { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add priovider
        /// </summary>
        /// <param name="messageQueueType">Message queue type</param>
        /// <param name="provider">Provider</param>
        /// <param name=""></param>
        public void AddProvider(MessageQueueType messageQueueType, ISixnetMessageQueueProvider provider)
        {
            if (provider != null)
            {
                _providers[messageQueueType] = provider;
            }
        }

        /// <summary>
        /// Get provider
        /// </summary>
        /// <param name="messageQueueType">Message queue type</param>
        /// <returns></returns>
        public ISixnetMessageQueueProvider GetProvider(MessageQueueType messageQueueType)
        {
            _providers.TryGetValue(messageQueueType, out var provider);
            return provider;
        }

        /// <summary>
        /// Get endpoint
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public MessageQueueEndpoint GetEndpoint(SixnetQueueMessage message)
        {
            return GetMessageQueueEndpoint(message) ?? Endpoint;
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ISixnetMessageQueueProvider> GetAllProviders()
        {
            return _providers.Values;
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="handler">Message handler</param>
        public void AddHandler(string topic, Func<SixnetQueueMessage, Task<bool>> handler)
        {
            if (string.IsNullOrWhiteSpace(topic) || handler == null)
            {
                return;
            }
            _messageHandlers[topic] = handler;
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="handler">Message handler</param>
        public void AddHandler(string topic, Func<SixnetQueueMessage, bool> handler)
        {
            if (string.IsNullOrWhiteSpace(topic) || handler == null)
            {
                return;
            }
            Task<bool> asyncHandler(SixnetQueueMessage msg)
            {
                var result = handler(msg);
                return Task.FromResult(result);
            }
            AddHandler(topic, asyncHandler);
        }

        /// <summary>
        /// Get handler
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <returns></returns>
        public Func<SixnetQueueMessage, Task<bool>> GetHandler(string topic)
        {
            if(string.IsNullOrWhiteSpace(topic))
            {
                return null;
            }
            _messageHandlers.TryGetValue(topic, out var handler);
            return handler;
        }

        #endregion
    }
}
