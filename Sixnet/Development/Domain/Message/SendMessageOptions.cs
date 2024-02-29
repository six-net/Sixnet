﻿using System.Collections.Generic;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Send message options
    /// </summary>
    public class SendMessageOptions
    {
        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public List<SixnetMessageInfo> Messages { get; set; }
    }
}
