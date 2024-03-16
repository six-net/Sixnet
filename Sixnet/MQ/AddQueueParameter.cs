using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.MQ
{
    /// <summary>
    /// Add queue parameter
    /// </summary>
    public class AddQueueParameter
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public MessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the queues
        /// </summary>
        public List<QueueInfo> Queues { get; set; }
    }
}
