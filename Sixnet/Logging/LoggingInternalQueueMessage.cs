using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sixnet.MQ;
using Sixnet.MQ.InProcess;

namespace Sixnet.Logging
{
    /// <summary>
    /// Defines log internal queue message
    /// </summary>
    internal class LoggingInternalQueueMessage : SixnetQueueMessage, IInternalQueueMessage
    {
        /// <summary>
        /// Gets or sets the log category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public EventId EventId { get; set; }

        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the args
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// Gets or sets the queue name
        /// </summary>
        public string QueueName { get; set; } = SixnetMQ.InternalQueueNames.Logging;

        public Task<bool> ExecuteAsync()
        {
            SixnetLogger.LogProvider?.WriteLog(CategoryName, Level, EventId, Exception, Message, Args);
            return Task.FromResult(true);
        }
    }
}
