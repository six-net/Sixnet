using System.Collections.Generic;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Send message context
    /// </summary>
    public class SendMessageContext
    {
        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<SixnetMessageInfo> Messages { get; set; }
    }
}
