using System;

namespace Sixnet.MQ
{
    /// <summary>
    /// Defines sixnet server queue message
    /// </summary>
    public class ServerQueueMessage : ISixnetQueueMessage
    {
        /// <summary>
        /// Gets or sets message id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets subject group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Whether persistent
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// Whether mandatory
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Return callback（must be set value if Mandatory value is True）
        /// </summary>
        public Func<MessageReturnRequest, MessageReturnResponse> ReturnCallback { get; set; }
    }
}
