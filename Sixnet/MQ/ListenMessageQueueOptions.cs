using System.Collections.Generic;

namespace Sixnet.MQ
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
