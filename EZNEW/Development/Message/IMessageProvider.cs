using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Development.Message
{
    /// <summary>
    /// Message provider contract
    /// </summary>
    public interface IMessageProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        SendMessageResult Send(SendMessageOptions options);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        Task<SendMessageResult> SendAsync(SendMessageOptions options);
    }
}
