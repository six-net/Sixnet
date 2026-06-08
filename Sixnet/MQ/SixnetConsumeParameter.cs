// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Consume parameter
    /// </summary>
    public class SixnetConsumeParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public SixnetMessageQueueServer Server { get; set; }

        /// <summary>
        /// Get or sets the queues
        /// </summary>
        public List<SixnetConsumeQueueEntry> Queues { get; set; }
    }
}
