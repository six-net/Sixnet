using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Subject message entry
    /// </summary>
    public class SubjectMessageEntry
    {
        /// <summary>
        /// Gets or sets the message subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Message infos
        /// </summary>
        public List<MessageInfo> MessageInfos { get; set; }
    }
}
