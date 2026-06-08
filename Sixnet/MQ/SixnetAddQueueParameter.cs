// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Add queue parameter
    /// </summary>
    public class SixnetAddQueueParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public SixnetMessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the queues
        /// </summary>
        public List<SixnetQueueInfo> Queues { get; set; }
    }
}
