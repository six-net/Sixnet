// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Delete queue parameter
    /// </summary>
    public class SixnetDeleteQueueParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public SixnetMessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the score
        /// </summary>
        public SixnetQueueScope Scope { get; set; } = SixnetQueueScope.Queues;

        /// <summary>
        /// Queue names
        /// </summary>
        public List<string> QueueNames { get; set; }
    }
}
