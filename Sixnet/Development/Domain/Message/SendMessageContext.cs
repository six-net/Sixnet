﻿using System.Collections.Generic;

namespace Sixnet.Development.Message
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
