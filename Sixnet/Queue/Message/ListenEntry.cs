using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Queue.Message
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
        /// Gets or sets the message process func
        /// </summary>
        public Func<string, bool> MessageProcessFunc { get; set; }
    }
}
