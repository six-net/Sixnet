using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Get message template context
    /// </summary>
    public class GetMessageTemplateContext
    {
        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the message subject
        /// </summary>
        public string MessageSubject { get; set; }
    }
}
