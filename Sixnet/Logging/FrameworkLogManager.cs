using System;
using System.Collections.Generic;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Work;
using Sixnet.Diagnostics;
using Sixnet.Serialization;
using Microsoft.Extensions.Logging;

namespace Sixnet.Logging
{
    /// <summary>
    /// Defines framework log manager
    /// </summary>
    public static class FrameworkLogManager
    {
        #region Fields

        const int SplitNum = 20;

        const char SplitChar = '=';

        static readonly string TitleTemplate = $"{new string(SplitChar, SplitNum)}{{0}}{new string(SplitChar, SplitNum)}";

        static readonly string NewLine = $"{Environment.NewLine}";

        internal static LogLevel LogLevel = LogLevel.Debug;

        static bool EnableTraceLog = false;

        #endregion

        static FrameworkLogManager()
        {
            EnableTraceLog = SixnetSwitches.ShouldTraceFramework(sw =>
            {
                EnableTraceLog = SixnetSwitches.ShouldTraceFramework();
            });
        }

        #region Util

        /// <summary>
        /// Framework log
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        internal static void Log(string categoryName, int eventId, string message)
        {
            SixnetLogger.Log(categoryName, LogLevel, eventId, $"{NewLine}{message}");
        }

        /// <summary>
        /// Get log title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        static string GetTitle(string title)
        {
            return string.Format(TitleTemplate, title);
        }

        /// <summary>
        /// Get log multiline message
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        static string GetMultilineMessage(params string[] messages)
        {
            if (messages.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return string.Join(NewLine, messages);
        } 

        #endregion

        #region UnitOfWork

        /// <summary>
        /// Log work start submitting
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkStartSubmitting(ISixnetWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】start submitting");
                Log(work.GetType().FullName, SixnetLogEvents.Work.StartSubmitting, GetMultilineMessage(title));
            }
        }

        /// <summary>
        /// Log work submitted successfully
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedSuccessfully(ISixnetWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】submit successfully");
                Log(work.GetType().FullName, SixnetLogEvents.Work.SubmittedSuccessfully, GetMultilineMessage(title, string.Empty));
            }
        }

        /// <summary>
        /// Log work submitted failure
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedFailure(ISixnetWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】submit failure");
                Log(work.GetType().FullName, SixnetLogEvents.Work.SubmittedFailure, GetMultilineMessage(title, string.Empty));
            }
        }

        /// <summary>
        /// Log work subbmited exception
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedException(ISixnetWork work, Exception ex)
        {
            if (EnableTraceLog && work != null && ex != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】submit error");
                Log(work.GetType().FullName, SixnetLogEvents.Work.SubmittedException, GetMultilineMessage(title, ex.Message));
            }
        }

        /// <summary>
        /// Log work rollback
        /// </summary>
        internal static void LogWorkRollback(ISixnetWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】rollback");
                Log(work.GetType().FullName, SixnetLogEvents.Work.Rollback, GetMultilineMessage(title));
            }
        }

        /// <summary>
        /// Log work dispose
        /// </summary>
        internal static void LogWorkDispose(ISixnetWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】dispose");
                Log(work.GetType().FullName, SixnetLogEvents.Work.Reset, GetMultilineMessage(title));
            }
        }

        #endregion

        #region Database script

        /// <summary>
        /// Log database script
        /// </summary>
        /// <param name="databaseProviderType">Database provider type</param>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        public static void LogDatabaseScript(Type databaseProviderType, DatabaseServerType databaseServerType, string script, object parameters)
        {
            if (EnableTraceLog && !string.IsNullOrWhiteSpace(script))
            {
                string title = GetTitle($"【{databaseServerType}】Execution script");
                //string content
                Log(databaseProviderType.FullName, SixnetLogEvents.Database.Script, GetMultilineMessage(GetTitle(title), script, SixnetJsonSerializer.Serialize(parameters)));
            }
        }

        /// <summary>
        /// Log database execution statement
        /// </summary>
        /// <param name="databaseProviderType">Database provider type</param>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="statement">Satement</param>
        public static void LogDatabaseExecutionStatement(Type databaseProviderType, DatabaseServerType databaseServerType, ExecutionDatabaseStatement statement)
        {
            if (EnableTraceLog && statement != null)
            {
                LogDatabaseScript(databaseProviderType, databaseServerType, statement.Script, statement.Parameters);
            }
        }

        #endregion
    }
}
