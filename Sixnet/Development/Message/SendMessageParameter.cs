using System.Collections.Generic;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Send message parameter
    /// </summary>
    public class SendMessageParameter
    {
        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<MessageInfo> Messages { get; set; }
    }
}
