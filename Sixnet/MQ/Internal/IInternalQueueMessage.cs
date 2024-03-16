using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.MQ.InProcess
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
