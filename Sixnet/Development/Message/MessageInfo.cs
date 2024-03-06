using System;
using System.Collections.Generic;
using Sixnet.Development.Work;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message info
    /// </summary>
    public class MessageInfo
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string Id { get; set; } = CreateMessageId();

        /// <summary>
        /// Gets or sets the work id
        /// </summary>
        public string WorkId { get; set; } = UnitOfWork.Current?.WorkId;

        /// <summary>
        /// Gets or sets the message subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public object Parameters { get; set; }

        /// <summary>
        /// Gets or sets the receivers
        /// Key => Message type
        /// Value => Receivers
        /// </summary>
        public Dictionary<string, List<string>> Receivers { get; set; }

        /// <summary>
        /// Create message id
        /// </summary>
        /// <returns></returns>
        public static string CreateMessageId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
