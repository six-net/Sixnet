// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Enqueue parameter
    /// </summary>
    public class EnqueueParameter
    {
        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        public MessageQueueEndpoint Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public SixnetQueueMessage Message { get; set; }
    }
}
