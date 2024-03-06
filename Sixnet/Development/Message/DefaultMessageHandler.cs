using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Default message handler
    /// </summary>
    internal class DefaultMessageHandler : ISixnetMessageHandler
    {
        private readonly Func<SendMessageContext, Task> _handler;

        internal DefaultMessageHandler(Func<SendMessageContext, Task> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns></returns>
        public Task SendAsync(SendMessageContext context)
        {
            return _handler?.Invoke(context);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="context">Send message context</param>
        /// <returns></returns>
        public void Send(SendMessageContext context)
        {
            _handler?.Invoke(context);
        }
    }
}
