using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

        #endregion

        #region Properties

        /// <summary>
        /// Get message queue server func
        /// </summary>
        public Func<ServerQueueMessage, MessageQueueServer> GetMessageQueueServer { get; set; }

        /// <summary>
        /// Gets or sets the default message server
        /// </summary>
        public MessageQueueServer Server { get; set; }

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
        /// Get server
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public MessageQueueServer GetServer(ServerQueueMessage message)
        {
            var server = GetMessageQueueServer(message);
            return server ?? Server;
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ISixnetMessageQueueProvider> GetAllProviders()
        {
            return _providers.Values;
        }

        #endregion
    }
}
