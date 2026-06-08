// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Send message context
    /// </summary>
    public class SixnetSendMessageContext
    {
        /// <summary>
        /// Gets or sets the message template
        /// </summary>
        public SixnetMessageTemplate Template { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public SixnetMessageInfo Message { get; set; }

        /// <summary>
        /// Gets or sets the receivers
        /// </summary>
        public List<string> Receivers { get; set; }
    }
}
