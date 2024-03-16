using System;
using System.Collections.Generic;
using Sixnet.Development.Event;
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
        public string Id { get; set; } = GenerateMessageId();

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
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the message send time
        /// </summary>
        public MessageSendTime SendTime { get; set; } = MessageSendTime.WorkCompleted;

        /// <summary>
        /// Gets or sets the create date
        /// </summary>
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;

        static string GenerateMessageId()
        {
            var msgOptions = SixnetMessager.GetMessageOptions();
            return msgOptions?.GetMessageId?.Invoke() ?? Guid.NewGuid().ToString();
        }
    }
}
