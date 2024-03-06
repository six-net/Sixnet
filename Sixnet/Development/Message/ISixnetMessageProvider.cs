using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message provider contract
    /// </summary>
    public interface ISixnetMessageProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="parameter">Send message parameter</param>
        /// <returns></returns>
        void Send(SendMessageParameter parameter);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="parameter">Send message parameter</param>
        /// <returns></returns>
        Task SendAsync(SendMessageParameter parameter);
    }
}
