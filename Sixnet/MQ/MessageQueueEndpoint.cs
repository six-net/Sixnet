using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.MQ
{
    /// <summary>
    /// Message queue endpoint
    /// </summary>
    public class MessageQueueEndpoint
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public MessageQueueServer Server { get; set; }

        /// <summary>
        /// Gets or sets the queue names
        /// </summary>
        public List<string> QueueNames { get; set; }
    }
}
