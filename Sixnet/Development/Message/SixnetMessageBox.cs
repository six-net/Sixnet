// "Company © 2025. All rights reserved."

using Sixnet.Development.Work;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message box
    /// </summary>
    internal class SixnetMessageBox
    {
        readonly List<SixnetMessageInfo> _messages = new();

        private SixnetMessageBox()
        {
            SixnetMessager.MessageBox?.Dispose();
            SixnetMessager.MessageBox = this;
        }

        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public IEnumerable<SixnetMessageInfo> Messages => _messages;

        /// <summary>
        /// Store message
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Store(IEnumerable<SixnetMessageInfo> messages)
        {
            if (!messages.IsNullOrEmpty())
            {
                foreach (SixnetMessageInfo message in messages)
                {
                    if (string.IsNullOrWhiteSpace(message.WorkId))
                    {
                        message.WorkId = SixnetUnitOfWork.Current?.WorkId;
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
        public static SixnetMessageBox Create()
        {
            return new SixnetMessageBox();
        }
    }
}
