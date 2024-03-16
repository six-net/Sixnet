namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue exchange type
    /// </summary>
    public enum MessageQueueExchangeType
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
    public enum MessageQueueType
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
    public enum QueueScope
    {
        Server = 100,
        Queues = 110
    }
}
