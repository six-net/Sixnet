using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Message handler contract
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        Task<SendMessageResult> SendAsync(SendMessageContext context);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        SendMessageResult Send(SendMessageContext context);
    }
}
