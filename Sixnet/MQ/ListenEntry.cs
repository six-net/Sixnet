using System;

namespace Sixnet.MQ
{
    /// <summary>
    /// Defines listen entry
    /// </summary>
    public class ListenEntry
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
        /// Gets or sets the message processer
        /// </summary>
        public Func<string, bool> MessageProcesser { get; set; }
    }
}
