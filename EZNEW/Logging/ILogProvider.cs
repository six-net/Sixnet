using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EZNEW.Logging
{
    /// <summary>
    /// Log provider contract
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Write log message
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void WriteLog(string loggerCategoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats the message and creates a scope.
        /// </summary>
        /// <param name="messageFormat">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A disposable scope object. Can be null</returns>
        IDisposable BeginScope(string messageFormat, params object[] args);
    }
}
