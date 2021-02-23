using EZNEW.Develop.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Message box
    /// </summary>
    internal class MessageBox
    {
        readonly List<MessageInfo> _messages = new List<MessageInfo>();

        private MessageBox()
        {
            MessageManager.CurrentMessageBox?.Dispose();
            MessageManager.CurrentMessageBox = this;
        }

        public IEnumerable<MessageInfo> Messages => _messages;

        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="newMessages">new messages</param>
        public void Add(params MessageInfo[] newMessages)
        {
            if (!newMessages.IsNullOrEmpty())
            {
                foreach (MessageInfo message in newMessages)
                {
                    if (string.IsNullOrWhiteSpace(message.WorkId))
                    {
                        message.WorkId = WorkManager.Current?.WorkId;
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
