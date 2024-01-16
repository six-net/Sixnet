using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Message queue exchange type
    /// </summary>
    public enum MessageQueueExchangeType
    {
        /// <summary>
        /// Direct
        /// </summary>
        Direct = 110,
        /// <summary>
        /// Fanout
        /// </summary>
        Fanout = 120,
        /// <summary>
        /// Topic
        /// </summary>
        Topic = 130,
        /// <summary>
        /// Headers
        /// </summary>
        Headers = 140
    }

    /// <summary>
    /// Message queue server type
    /// </summary>
    public enum MessageQueueServerType
    {
        /// <summary>
        /// RabbmitMQ
        /// </summary>
        RabbmitMQ = 210
    }
}
