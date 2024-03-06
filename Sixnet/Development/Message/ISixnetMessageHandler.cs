using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message handler contract
    /// </summary>
    public interface ISixnetMessageHandler
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        Task SendAsync(SendMessageContext context);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns>Return send result</returns>
        void Send(SendMessageContext context);
    }
}
