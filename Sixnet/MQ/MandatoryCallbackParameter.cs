using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.MQ
{
    /// <summary>
    /// Mandatory callback parameter
    /// </summary>
    public class MandatoryCallbackParameter
    {
        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        public MessageQueueEndpoint Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public SixnetQueueMessage Message { get; set; }
    }
}
