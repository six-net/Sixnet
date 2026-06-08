// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue exchange type
    /// </summary>
    public enum SixnetMessageQueueExchangeType
    {
        /// <summary>
        /// Direct
        /// </summary>
        Direct = 110,
        /// <summary>
        /// Fanout
        /// </summary>
        Fanout = 120,
        /// <summary>
        /// Topic
        /// </summary>
        Topic = 130,
        /// <summary>
        /// Headers
        /// </summary>
        Headers = 140
    }

    /// <summary>
    /// Message queue type
    /// </summary>
    public enum SixnetMessageQueueType
    {
        /// <summary>
        /// Internal
        /// </summary>
        Internal = 200,
        /// <summary>
        /// RabbmitMQ
        /// </summary>
        RabbmitMQ = 210,
        /// <summary>
        /// Kafka
        /// </summary>
        Kafka = 220,
        /// <summary>
        /// Others
        /// </summary>
        Others = 500
    }

    /// <summary>
    /// Queue scope
    /// </summary>
    public enum SixnetQueueScope
    {
        Server = 100,
        Queues = 110
    }
}
