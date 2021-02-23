using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Send message context
    /// </summary>
    public class SendMessageContext
    {
        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<MessageInfo> Messages { get; set; }
    }
}
