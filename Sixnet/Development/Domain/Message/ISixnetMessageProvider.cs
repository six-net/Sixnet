using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Message provider contract
    /// </summary>
    public interface ISixnetMessageProvider
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
