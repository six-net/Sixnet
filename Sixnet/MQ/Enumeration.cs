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
        /// RabbmitMQ
        /// </summary>
        RabbmitMQ = 210,
        Kafka = 220,
        Others
    }
}
