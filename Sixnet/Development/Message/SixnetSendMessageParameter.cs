// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Send message parameter
    /// </summary>
    public class SixnetSendMessageParameter
    {
        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<SixnetMessageInfo> Messages { get; set; }
    }
}
