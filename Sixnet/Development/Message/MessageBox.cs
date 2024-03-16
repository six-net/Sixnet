using System.Collections.Generic;
using Sixnet.Development.Work;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message box
    /// </summary>
    internal class MessageBox
    {
        readonly List<MessageInfo> _messages = new();

        private MessageBox()
        {
            SixnetMessager.MessageBox?.Dispose();
            SixnetMessager.MessageBox = this;
        }

        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<MessageInfo> Messages => _messages;

        /// <summary>
        /// Store message
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Store(IEnumerable<MessageInfo> messages)
        {
            if (!messages.IsNullOrEmpty())
            {
                foreach (MessageInfo message in messages)
                {
                    if (string.IsNullOrWhiteSpace(message.WorkId))
                    {
                        message.WorkId = UnitOfWork.Current?.WorkId;
                    }
                }
                _messages.AddRange(messages);
            }
        }

        /// <summary>
        /// Clear message
        /// </summary>
        public void Clear()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Create message box
        /// </summary>
        /// <returns></returns>
        public static MessageBox Create()
        {
            return new MessageBox();
        }
    }
}
