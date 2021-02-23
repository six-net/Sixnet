using EZNEW.Fault;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Default message handler
    /// </summary>
    internal class DefaultMessageHandler : IMessageHandler
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
        public async Task<SendMessageResult> SendAsync(SendMessageContext context)
        {
            if (HandlerFunc == null)
            {
                throw new ArgumentNullException(nameof(HandlerFunc));
            }
            return await HandlerFunc(context).ConfigureAwait(false);
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
