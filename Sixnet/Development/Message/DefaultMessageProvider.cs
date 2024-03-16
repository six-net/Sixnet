using Sixnet.MQ;
using Sixnet.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Default message provider
    /// </summary>
    internal class DefaultMessageProvider : ISixnetMessageProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="parameter">Send message parameter</param>
        /// <returns>Return send result</returns>
        public void Send(SendMessageParameter parameter)
        {
            if (parameter?.Messages.IsNullOrEmpty() ?? true)
            {
                return;
            }
            var queueMessages = parameter.Messages.Select(msg =>
            {
                return new SixnetQueueMessage()
                {
                    Topic = msg.Subject,
                    Group = SixnetMQ.QueueMessageGroupNames.DomainMessage,
                    Id = msg.Id,
                    Content = SixnetJsonSerializer.Serialize(msg),
                };
            }).ToList();
            SixnetMQ.Enqueue(queueMessages);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        public Task SendAsync(SendMessageParameter parameter)
        {
            if (parameter?.Messages.IsNullOrEmpty() ?? true)
            {
                return Task.CompletedTask;
            }
            var queueMessages = parameter.Messages.Select(msg =>
            {
                return new SixnetQueueMessage()
                {
                    Topic = msg.Subject,
                    Group = string.Empty,
                    Id = msg.Id,
                    Content = SixnetJsonSerializer.Serialize(msg),
                };
            }).ToList();
            return SixnetMQ.EnqueueAsync(queueMessages);
        }
    }
}
