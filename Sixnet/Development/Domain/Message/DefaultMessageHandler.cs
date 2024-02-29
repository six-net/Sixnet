using System;
using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Default message handler
    /// </summary>
    internal class DefaultMessageHandler : ISixnetMessageHandler
    {
        private Func<SendMessageContext, Task<SendMessageResult>> HandlerFunc { get; set; }

        internal DefaultMessageHandler(Func<SendMessageContext, Task<SendMessageResult>> handlerFunc)
        {
            HandlerFunc = handlerFunc;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        public Task<SendMessageResult> SendAsync(SendMessageContext context)
        {
            if (HandlerFunc == null)
            {
                throw new ArgumentNullException(nameof(HandlerFunc));
            }
            return HandlerFunc(context);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        public SendMessageResult Send(SendMessageContext context)
        {
            return SendAsync(context).Result;
        }
    }
}
