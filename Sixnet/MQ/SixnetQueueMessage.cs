namespace Sixnet.MQ
{
    /// <summary>
    /// Queue message
    /// </summary>
    public class SixnetQueueMessage
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the message topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the message content
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
    }
}
