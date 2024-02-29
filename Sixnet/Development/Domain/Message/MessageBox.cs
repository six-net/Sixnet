using System.Collections.Generic;
using Sixnet.Development.Work;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Message box
    /// </summary>
    internal class MessageBox
    {
        readonly List<SixnetMessageInfo> _messages = new List<SixnetMessageInfo>();

        private MessageBox()
        {
            SixnetMessager.MessageBox?.Dispose();
            SixnetMessager.MessageBox = this;
        }

        public IEnumerable<SixnetMessageInfo> Messages => _messages;

        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="newMessages">new messages</param>
        public void Add(params SixnetMessageInfo[] newMessages)
        {
            if (!newMessages.IsNullOrEmpty())
            {
                foreach (SixnetMessageInfo message in newMessages)
                {
                    if (string.IsNullOrWhiteSpace(message.WorkId))
                    {
                        message.WorkId = UnitOfWork.Current?.WorkId;
                    }
                }
                _messages.AddRange(newMessages);
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
