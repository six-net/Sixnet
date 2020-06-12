using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EZNEW.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EZNEW.Logging
{
    /// <summary>
    /// Default log provider
    /// </summary>
    internal class DefaultLogProvider : ILogProvider
    {
        /// <summary>
        /// Logger factory
        /// </summary>
        private static readonly ILoggerFactory LoggerFactory = null;

        static DefaultLogProvider()
        {
            LoggerFactory = ContainerManager.Resolve<ILoggerFactory>();
        }

        #region Gets logger

        /// <summary>
        /// Gets logger
        /// </summary>
        /// <param name="loggerCategoryName">Logger category name</param>
        /// <returns>Return the logger</returns>
        static ILogger GetLogger(string loggerCategoryName)
        {
            if (LoggerFactory == null)
            {
                return null;
            }
            loggerCategoryName = string.IsNullOrWhiteSpace(loggerCategoryName) ? LogManager.DefaultLoggerCategoryName : loggerCategoryName;
            return LoggerFactory.CreateLogger(loggerCategoryName);
        }

        #endregion

        #region Write log message

        /// <summary>
        /// Write log message
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLog(string loggerCategoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            var logger = GetLogger(loggerCategoryName);
            if (logger != null)
            {
                logger.Log(logLevel, eventId, exception, message, args);
            }
        }

        #endregion

        #region Formats the message and creates a scope.

        /// <summary>
        /// Formats the message and creates a scope.
        /// </summary>
        /// <param name="messageFormat">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A disposable scope object. Can be null</returns>
        public IDisposable BeginScope(string messageFormat, params object[] args)
        {
            ILogger logger = GetLogger(string.Empty);
            if (logger != null)
            {
                return logger.BeginScope(messageFormat, args);
            }
            return null;
        }

        #endregion
    }
}
