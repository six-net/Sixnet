using System.Collections.Generic;

namespace Sixnet.MQ
{
    /// <summary>
    /// Consume parameter
    /// </summary>
    public class ConsumeParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public MessageQueueServer Server { get; set; }

        /// <summary>
        /// Get or sets the queues
        /// </summary>
        public List<ConsumeQueueEntry> Queues { get; set; }
    }
}
