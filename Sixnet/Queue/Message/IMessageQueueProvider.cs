using System.Threading.Tasks;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Defines message queue provider
    /// </summary>
    public interface IMessageQueueProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Message options</param>
        /// <returns>response</returns>
        Task SendAsync(MessageQueueServer server, SendMessageOptions options);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Message options</param>
        /// <returns>response</returns>
        void Send(MessageQueueServer server, SendMessageOptions options);

        /// <summary>
        /// Listen message queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="listenOptions">Listen options</param>
        Task ListenAsync(MessageQueueServer server, ListenMessageQueueOptions listenOptions);

        /// <summary>
        /// Listen message queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="listenOptions">Listen options</param>
        void Listen(MessageQueueServer server, ListenMessageQueueOptions listenOptions);

        /// <summary>
        /// Unlisten server
        /// </summary>
        /// <param name="servers">Servers</param>
        Task UnlistenServerAsync(params MessageQueueServer[] servers);

        /// <summary>
        /// Unlisten server
        /// </summary>
        /// <param name="servers">Servers</param>
        void UnlistenServer(params MessageQueueServer[] servers);

        /// <summary>
        /// Unlisten queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="queueNames">Queue names</param>
        Task UnlistenQueueAsync(MessageQueueServer server, params string[] queueNames);

        /// <summary>
        /// Unlisten queue
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="queueNames">Queue names</param>
        void UnlistenQueue(MessageQueueServer server, params string[] queueNames);

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        Task UnlistenAllAsync();

        /// <summary>
        /// Unlisten all queue
        /// </summary>
        void UnlistenAll();
    }
}
