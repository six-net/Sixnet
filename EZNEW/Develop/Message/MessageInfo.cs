using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Message info
    /// </summary>
    public class MessageInfo
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the work id
        /// </summary>
        public string WorkId { get; set; } = WorkManager.Current?.WorkId;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public object Parameters { get; set; }

        /// <summary>
        /// Gets or sets the receivers
        /// Key => Receiver type
        /// Value => Receiver values
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
