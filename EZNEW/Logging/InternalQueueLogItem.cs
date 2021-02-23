using System;
using EZNEW.Queue;
using Microsoft.Extensions.Logging;

namespace EZNEW.Logging
{
    /// <summary>
    /// Write log internal queue item
    /// </summary>
    public class InternalQueueLogItem : IInternalQueueItem
    {
        /// <summary>
        /// Gets or sets the log category name
        /// </summary>
        public string LogCategoryName { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public LogLevel LogLevel { get; set; }

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

        public void Execute() => LogManager.LogProvider?.WriteLog(LogCategoryName, LogLevel, EventId, Exception, Message, Args);
    }
}
