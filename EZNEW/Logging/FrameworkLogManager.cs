using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using EZNEW.Development.UnitOfWork;
using EZNEW.Diagnostics;
using EZNEW.Development.Command;
using EZNEW.Serialization;
using EZNEW.Data;

namespace EZNEW.Logging
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
            EnableTraceLog = SwitchManager.ShouldTraceFramework(sw =>
            {
                EnableTraceLog = SwitchManager.ShouldTraceFramework();
            });
        }

        internal static void Log(string categoryName, int eventId, string message)
        {
            LogManager.Log(categoryName, LogLevel, eventId, $"{NewLine}{message}");
        }

        static string GetTitle(string title)
        {
            return string.Format(TitleTemplate, title);
        }

        static string GetMultilineMessage(params string[] messages)
        {
            if (messages.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return string.Join(NewLine, messages);
        }

        #region UnitOfWork

        /// <summary>
        /// Log work start submitting
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkStartSubmitting(IWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Start submitting");
                Log(work.GetType().FullName, FrameworkLogEvents.Work.StartSubmitting, GetMultilineMessage(title));
            }
        }

        /// <summary>
        /// Log work options
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="options">Execution options</param>
        internal static void LogWorkOptions(IWork work, CommandExecutionOptions options = null)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Execution options");
                string content = options == null ? string.Empty : JsonSerializer.Serialize(options);
                Log(work.GetType().FullName, FrameworkLogEvents.Work.ExecutionOptions, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log work submitted successfully
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedSuccessfully(IWork work, WorkCommitResult commitResult)
        {
            if (EnableTraceLog && work != null && commitResult != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Submitted successfully");
                string content = $"Total command count：{commitResult.CommittedCommandCount},Affected data count：{commitResult.AffectedDataCount},Allow empty result command count：{commitResult.AllowEmptyCommandCount}";
                Log(work.GetType().FullName, FrameworkLogEvents.Work.SubmittedSuccessfully, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log work submitted failure
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedFailure(IWork work, WorkCommitResult commitResult)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Submitted failure");
                string content = $"Total command count：{commitResult.CommittedCommandCount},Affected data count：{commitResult.AffectedDataCount},Allow empty result command count：{commitResult.AllowEmptyCommandCount}";
                Log(work.GetType().FullName, FrameworkLogEvents.Work.SubmittedFailure, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log work subbmited exception
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void LogWorkSubmittedException(IWork work, Exception ex)
        {
            if (EnableTraceLog && work != null && ex != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Submitted Exception");
                Log(work.GetType().FullName, FrameworkLogEvents.Work.SubmittedException, GetMultilineMessage(title, ex.Message));
            }
        }

        /// <summary>
        /// Log work rollback
        /// </summary>
        internal static void LogWorkRollback(IWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Rollback");
                Log(work.GetType().FullName, FrameworkLogEvents.Work.Rollback, GetMultilineMessage(title));
            }
        }

        /// <summary>
        /// Log work reset
        /// </summary>
        internal static void LogWorkReset(IWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Reset");
                Log(work.GetType().FullName, FrameworkLogEvents.Work.Reset, GetMultilineMessage(title));
            }
        }

        /// <summary>
        /// Log work dispose
        /// </summary>
        internal static void LogWorkDispose(IWork work)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Dispose");
                Log(work.GetType().FullName, FrameworkLogEvents.Work.Reset, GetMultilineMessage(title));
            }
        }

        #endregion

        #region Activation record & Command

        /// <summary>
        /// Obsolete activation record
        /// </summary>
        /// <param name="activationRecord">Activation record</param>
        /// <param name="command">Command</param>
        internal static void ObsoleteActivationRecord(IWork work, IActivationRecord activationRecord, ICommand command)
        {
            if (EnableTraceLog && work != null && activationRecord != null)
            {
                string message = $"【Work:{work.WorkId}】【Record:{activationRecord.IdentityValue}】{(command == null ? "No execution command was generated" : $"The execution command is obsolete")}";
                Log(work.GetType().FullName, FrameworkLogEvents.ActivationRecord.Obsolete, message);
            }
        }

        /// <summary>
        /// Break activation record
        /// </summary>
        /// <param name="activationRecord">Activation record</param>
        /// <param name="command">Command</param>
        internal static void BreakActivationRecord(IWork work, IActivationRecord activationRecord, ICommand command)
        {
            if (EnableTraceLog && work != null && activationRecord != null && command != null)
            {
                string message = $"【Work:{work.WorkId}】【Record:{activationRecord.IdentityValue}】【Command:{command.Id}】 is break off by the command startting event handler";
                Log(work.GetType().FullName, FrameworkLogEvents.ActivationRecord.Break, message);
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
                Log(databaseProviderType.FullName, FrameworkLogEvents.Database.Script, GetMultilineMessage(GetTitle(title), script, JsonSerializer.Serialize(parameters)));
            }
        }

        /// <summary>
        /// Log database execution command
        /// </summary>
        /// <param name="databaseProviderType">Database provider type</param>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="cmd">Command</param>
        public static void LogDatabaseExecutionCommand(Type databaseProviderType, DatabaseServerType databaseServerType, DatabaseExecutionCommand cmd)
        {
            if (EnableTraceLog && cmd != null)
            {
                LogDatabaseScript(databaseProviderType, databaseServerType, cmd.CommandText, cmd.Parameters);
            }
        }

        #endregion
    }
}
