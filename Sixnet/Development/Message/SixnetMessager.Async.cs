using Sixnet.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    public static partial class SixnetMessager
    {
        #region Send message

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static Task SendAsync(params MessageInfo[] messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var parameter = new SendMessageParameter()
            {
                Messages = new List<MessageInfo>(messages)
            };
            var messageProvider = GetMessageProvider();
            return messageProvider.SendAsync(parameter);
        }

        #endregion

        #region Commit message

        /// <summary>
        /// Commit stored messages
        /// </summary>
        /// <returns>Return send result</returns>
        internal static async Task CommitAsync()
        {
            if (MessageBox?.Messages?.IsNullOrEmpty() ?? true)
            {
                return;
            }
            var messageProvider = GetMessageProvider();
            await messageProvider.SendAsync(new SendMessageParameter()
            {
                Messages = MessageBox.Messages
            }).ConfigureAwait(false);
            MessageBox.Clear();
        }

        #endregion
    }
}
