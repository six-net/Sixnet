using System;

namespace Sixnet.MQ
{
    /// <summary>
    /// Listen queue entry
    /// </summary>
    public class ConsumeQueueEntry
    {
        /// <summary>
        /// Gets or sets the queue name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the listen count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the prefetch message count
        /// </summary>
        public int PrefetchCount { get; set; }

        /// <summary>
        /// Message whether recover
        /// </summary>
        public bool Recover { get; set; }

        /// <summary>
        /// Gets or sets the message handler
        /// </summary>
        public Func<string, bool> MessageHandler { get; set; }
    }
}
