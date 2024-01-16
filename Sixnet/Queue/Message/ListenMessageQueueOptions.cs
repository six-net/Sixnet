using System.Collections.Generic;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Defines listen message queue options
    /// </summary>
    public class ListenMessageQueueOptions
    {
        /// <summary>
        /// Get or sets the message listen entries
        /// </summary>
        public List<ListenEntry> Entries { get; set; }
    }
}
