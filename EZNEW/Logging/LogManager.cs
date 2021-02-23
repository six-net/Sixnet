using System;
using Microsoft.Extensions.Logging;
using EZNEW.DependencyInjection;
using EZNEW.Queue;

namespace EZNEW.Logging
{
    /// <summary>
    /// Log manager
    /// </summary>
    public static class LogManager
    {
        #region Fields

        /// <summary>
        /// Gets or sets the default logger catgory name.
        /// Default value is empty
        /// </summary>
        public static string DefaultLoggerCategoryName { get; set; } = "eznew";

        /// <summary>
        /// Log provider
        /// </summary>
        internal static readonly ILogProvider LogProvider = null;

        #endregion

        static LogManager()
        {
            LogProvider = ContainerManager.Resolve<ILogProvider>();
            if (LogProvider == null)
            {
                LogProvider = new DefaultLogProvider();
            }
        }

        #region Write log message

        /// <summary>
        /// Write log message
        /// </summary>
        /// <param name="categoryType">The logger categoryType,Can be null</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        static void WriteLog(Type categoryType, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteLog(categoryType?.FullName ?? string.Empty, logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Write log message
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        static void WriteLog(string loggerCategoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            var writeLogCommand = new InternalQueueLogItem()
            {
                LogCategoryName = loggerCategoryName,
                LogLevel = logLevel,
                EventId = eventId,
                Exception = exception,
                Message = message,
                Args = args
            };
            InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.Logging).Enqueue(writeLogCommand);
        }

        #endregion

        #region Formats the message and creates a scope

        /// <summary>
        /// Formats the message and creates a scope.
        /// </summary>
        /// <param name="messageFormat">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A disposable scope object. Can be null</returns>
        public static IDisposable BeginScope(string messageFormat, params object[] args)
        {
            return LogProvider?.BeginScope(messageFormat, args);
        }

        #endregion

        #region Log

        #region Log by default category

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteLog(string.Empty, logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            Log(logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            Log(logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(LogLevel logLevel, string message, params object[] args)
        {
            Log(logLevel, 0, null, message, args);
        }

        #endregion

        #region Log by category name

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(string loggerCategoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteLog(loggerCategoryName, logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(string loggerCategoryName, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            Log(loggerCategoryName, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(string loggerCategoryName, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(string loggerCategoryName, LogLevel logLevel, string message, params object[] args)
        {
            Log(loggerCategoryName, logLevel, 0, null, message, args);
        }

        #endregion

        #region Log by category type

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(Type loggerCategoryType, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteLog(loggerCategoryType, logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(Type loggerCategoryType, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            Log(loggerCategoryType, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(Type loggerCategoryType, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log(Type loggerCategoryType, LogLevel logLevel, string message, params object[] args)
        {
            Log(loggerCategoryType, logLevel, 0, null, message, args);
        }

        #endregion

        #region Log by generic category type

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log<TCategory>(LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(typeof(TCategory), logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log<TCategory>(LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            Log<TCategory>(logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log<TCategory>(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Log<TCategory>(LogLevel logLevel, string message, params object[] args)
        {
            Log<TCategory>(logLevel, 0, null, message, args);
        }

        #endregion

        #region LogIf by default category

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (condition)
            {
                WriteLog(string.Empty, logLevel, eventId, exception, message, args);
            }
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            LogIf(condition, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            LogIf(condition, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, LogLevel logLevel, string message, params object[] args)
        {
            LogIf(condition, logLevel, 0, null, message, args);
        }

        #endregion

        #region LogIf by category name

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, string loggerCategoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (condition)
            {
                WriteLog(loggerCategoryName, logLevel, eventId, exception, message, args);
            }
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, string loggerCategoryName, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, string loggerCategoryName, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, string loggerCategoryName, LogLevel logLevel, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, logLevel, 0, null, message, args);
        }

        #endregion

        #region LogIf by category type

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, Type loggerCategoryType, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (condition)
            {
                WriteLog(loggerCategoryType, logLevel, eventId, exception, message, args);
            }
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, Type loggerCategoryType, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, Type loggerCategoryType, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf(bool condition, Type loggerCategoryType, LogLevel logLevel, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, logLevel, 0, null, message, args);
        }

        #endregion

        #region LogIf by generic category type

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf<TCategory>(bool condition, LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, typeof(TCategory), logLevel, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf<TCategory>(bool condition, LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            LogIf<TCategory>(condition, logLevel, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf<TCategory>(bool condition, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, logLevel, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a log message at the specified log level.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogIf<TCategory>(bool condition, LogLevel logLevel, string message, params object[] args)
        {
            LogIf<TCategory>(condition, logLevel, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogCritical

        #region  Log critical by default

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(EventId eventId, string message, params object[] args)
        {
            LogCritical(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(Exception exception, string message, params object[] args)
        {
            LogCritical(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(string message, params object[] args)
        {
            LogCritical(0, null, message, args);
        }

        #endregion

        #region Log critical by category name

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogCritical(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogCritical(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(string loggerCategoryName, string message, params object[] args)
        {
            LogCritical(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log critical by category type

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogCritical(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogCritical(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(Type loggerCategoryType, string message, params object[] args)
        {
            LogCritical(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log critical by generic category type

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogCritical<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical<TCategory>(Exception exception, string message, params object[] args)
        {
            LogCritical<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical<TCategory>(string message, params object[] args)
        {
            LogCritical<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log critical with condition by default

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogCriticalIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogCriticalIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, string message, params object[] args)
        {
            LogCriticalIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log critical with condition by category name

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log critical with condition by category type

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogCriticalIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log critical with condition by generic category type

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogCriticalIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogCriticalIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCriticalIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogCriticalIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogDebug

        #region Log debug by default

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(EventId eventId, string message, params object[] args)
        {
            LogDebug(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(Exception exception, string message, params object[] args)
        {
            LogDebug(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(string message, params object[] args)
        {
            LogDebug(0, null, message, args);
        }

        #endregion

        #region Log debug by category name

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogDebug(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogDebug(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(string loggerCategoryName, string message, params object[] args)
        {
            LogDebug(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log debug by category type

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogDebug(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogDebug(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug(Type loggerCategoryType, string message, params object[] args)
        {
            LogDebug(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log debug by generic category type

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogDebug<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug<TCategory>(Exception exception, string message, params object[] args)
        {
            LogDebug<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebug<TCategory>(string message, params object[] args)
        {
            LogDebug<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log debug with condition by default

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogDebugIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogDebugIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, string message, params object[] args)
        {
            LogDebugIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log debug with condition by category name

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log debug with condition by category type

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogDebugIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log debug with condition by generic category type

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogDebugIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogDebugIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogDebugIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogDebugIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogError

        #region Log error by default

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(EventId eventId, string message, params object[] args)
        {
            LogError(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(Exception exception, string message, params object[] args)
        {
            LogError(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(string message, params object[] args)
        {
            LogError(0, null, message, args);
        }

        #endregion

        #region Log error by category name

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogError(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogError(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(string loggerCategoryName, string message, params object[] args)
        {
            LogError(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log error by category type

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogError(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogError(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(Type loggerCategoryType, string message, params object[] args)
        {
            LogError(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log error by generic category type

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogError<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError<TCategory>(Exception exception, string message, params object[] args)
        {
            LogError<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError<TCategory>(string message, params object[] args)
        {
            LogError<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log error with condition by default

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogErrorIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogErrorIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, string message, params object[] args)
        {
            LogErrorIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log error with condition by category name

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log error with condition by category type

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogErrorIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log error with condition by generic category type

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogErrorIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogErrorIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogErrorIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogErrorIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogInformation

        #region Log information by default

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(EventId eventId, string message, params object[] args)
        {
            LogInformation(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(Exception exception, string message, params object[] args)
        {
            LogInformation(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(string message, params object[] args)
        {
            LogInformation(0, null, message, args);
        }

        #endregion

        #region Log information by category name

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogInformation(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogInformation(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(string loggerCategoryName, string message, params object[] args)
        {
            LogInformation(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log information by category type

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogInformation(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogInformation(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(Type loggerCategoryType, string message, params object[] args)
        {
            LogInformation(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log information by generic category type

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogInformation<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation<TCategory>(Exception exception, string message, params object[] args)
        {
            LogInformation<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation<TCategory>(string message, params object[] args)
        {
            LogInformation<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log information with condition by default

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogInformationIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogInformationIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, string message, params object[] args)
        {
            LogInformationIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log information with condition by category name

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log information with condition by category type

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogInformationIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log information with condition by generic category type

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogInformationIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogInformationIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformationIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogInformationIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogTrace

        #region Log trace by default

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(EventId eventId, string message, params object[] args)
        {
            LogTrace(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(Exception exception, string message, params object[] args)
        {
            LogTrace(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(string message, params object[] args)
        {
            LogTrace(0, null, message, args);
        }

        #endregion

        #region Log trace by category name

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogTrace(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogTrace(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(string loggerCategoryName, string message, params object[] args)
        {
            LogTrace(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log trace by category type

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogTrace(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogTrace(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(Type loggerCategoryType, string message, params object[] args)
        {
            LogTrace(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log trace by generic category type

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogTrace<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace<TCategory>(Exception exception, string message, params object[] args)
        {
            LogTrace<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace<TCategory>(string message, params object[] args)
        {
            LogTrace<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log trace with condition by default

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogTraceIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogTraceIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, string message, params object[] args)
        {
            LogTraceIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log trace with condition by category name

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log trace with condition by category type

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogTraceIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log trace with condition by generic category type

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogTraceIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogTraceIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTraceIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogTraceIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion

        #region LogWarning

        #region Log warning by default

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(EventId eventId, string message, params object[] args)
        {
            LogWarning(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(Exception exception, string message, params object[] args)
        {
            LogWarning(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(string message, params object[] args)
        {
            LogWarning(0, null, message, args);
        }

        #endregion

        #region Log warning by category name

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryName, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogWarning(loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogWarning(loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(string loggerCategoryName, string message, params object[] args)
        {
            LogWarning(loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log warning by category type

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            Log(loggerCategoryType, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogWarning(loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogWarning(loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(Type loggerCategoryType, string message, params object[] args)
        {
            LogWarning(loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log warning by generic category type

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning<TCategory>(EventId eventId, Exception exception, string message, params object[] args)
        {
            Log<TCategory>(LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning<TCategory>(EventId eventId, string message, params object[] args)
        {
            LogWarning<TCategory>(eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning<TCategory>(Exception exception, string message, params object[] args)
        {
            LogWarning<TCategory>(0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning<TCategory>(string message, params object[] args)
        {
            LogWarning<TCategory>(0, null, message, args);
        }

        #endregion

        #region Log warning with condition by default

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, EventId eventId, string message, params object[] args)
        {
            LogWarningIf(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, Exception exception, string message, params object[] args)
        {
            LogWarningIf(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, string message, params object[] args)
        {
            LogWarningIf(condition, 0, null, message, args);
        }

        #endregion

        #region Log warning with condition by category name

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, string loggerCategoryName, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryName, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, string loggerCategoryName, EventId eventId, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryName, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, string loggerCategoryName, Exception exception, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryName, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryName">The logger category name.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, string loggerCategoryName, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryName, 0, null, message, args);
        }

        #endregion

        #region Log warning with condition by category type

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, Type loggerCategoryType, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf(condition, loggerCategoryType, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, Type loggerCategoryType, EventId eventId, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryType, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, Type loggerCategoryType, Exception exception, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryType, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="loggerCategoryType">The logger category type.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf(bool condition, Type loggerCategoryType, string message, params object[] args)
        {
            LogWarningIf(condition, loggerCategoryType, 0, null, message, args);
        }

        #endregion

        #region Log warning with condition by generic category type

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf<TCategory>(bool condition, EventId eventId, Exception exception, string message, params object[] args)
        {
            LogIf<TCategory>(condition, LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf<TCategory>(bool condition, EventId eventId, string message, params object[] args)
        {
            LogWarningIf<TCategory>(condition, eventId, null, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf<TCategory>(bool condition, Exception exception, string message, params object[] args)
        {
            LogWarningIf<TCategory>(condition, 0, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <typeparam name="TCategory">The logger categoryType</typeparam>
        /// <param name="condition">Whether to perform logging</param>
        /// <param name="message">Format string of the log message in message template format. Example:"User {User} logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarningIf<TCategory>(bool condition, string message, params object[] args)
        {
            LogWarningIf<TCategory>(condition, 0, null, message, args);
        }

        #endregion

        #endregion
    }
}
