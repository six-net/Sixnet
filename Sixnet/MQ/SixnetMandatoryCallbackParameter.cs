// "Company © 2025. All rights reserved."

namespace Sixnet.MQ
{
    /// <summary>
    /// Mandatory callback parameter
    /// </summary>
    public class SixnetMandatoryCallbackParameter
    {
        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        public SixnetMessageQueueEndpoint Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public SixnetQueueMessage Message { get; set; }
    }
}
