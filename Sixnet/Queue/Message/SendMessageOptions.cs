using System;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Defines send message options
    /// </summary>
    public class SendMessageOptions
    {
        /// <summary>
        /// Gets or sets subject group
        /// </summary>
        public string SubjectGroup { get; set; }

        /// <summary>
        /// Gets or sets subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Whether mandatory
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// whether persistent
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// Return callback（must be set value if Mandatory value is True）
        /// </summary>
        public Func<MessageReturnRequest, MessageReturnResponse> ReturnCallback { get; set; }
    }
}
