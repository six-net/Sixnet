// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Abort consume parameter
    /// </summary>
    public class AbortConsumeParameter
    {
        /// <summary>
        /// Gets or sets the message queue server
        /// </summary>
        public MessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the queue names
        /// </summary>
        public List<string> QueueNames { get; set; }

        /// <summary>
        /// Gets or sets the abort consume target
        /// </summary>
        public QueueScope Scope { get; set; } = QueueScope.Queues;
    }
}
