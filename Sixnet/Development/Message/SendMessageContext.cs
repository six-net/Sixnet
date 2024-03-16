using System.Collections.Generic;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Send message context
    /// </summary>
    public class SendMessageContext
    {
        /// <summary>
        /// Gets or sets the message template
        /// </summary>
        public MessageTemplate Template { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public MessageInfo Message { get; set; }

        /// <summary>
        /// Gets or sets the receivers
        /// </summary>
        public List<string> Receivers { get; set; }
    }
}
