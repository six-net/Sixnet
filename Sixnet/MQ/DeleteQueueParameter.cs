// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Delete queue parameter
    /// </summary>
    public class DeleteQueueParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public MessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the score
        /// </summary>
        public QueueScope Scope { get; set; } = QueueScope.Queues;

        /// <summary>
        /// Queue names
        /// </summary>
        public List<string> QueueNames { get; set; }
    }
}
