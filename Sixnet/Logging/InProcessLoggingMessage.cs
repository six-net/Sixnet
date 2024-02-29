using System;
using Microsoft.Extensions.Logging;
using Sixnet.MQ;

namespace Sixnet.Logging
{
    /// <summary>
    /// Defines log internal queue task
    /// </summary>
    public class InProcessLoggingMessage : InProcessQueueMessage
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
        /// Execute write log
        /// </summary>
        public override void Execute() => SixnetLogger.LogProvider?.WriteLog(CategoryName, Level, EventId, Exception, Message, Args);
    }
}
