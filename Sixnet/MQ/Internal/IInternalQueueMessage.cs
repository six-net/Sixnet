// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.MQ.Internal
{
    /// <summary>
    /// Internal queue message
    /// </summary>
    public interface IInternalQueueMessage
    {
        string QueueName { get; set; }

        Task<bool> ExecuteAsync();
    }
}
