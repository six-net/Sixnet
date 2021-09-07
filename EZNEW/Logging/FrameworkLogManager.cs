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

        static readonly string TitleLogTemplate = $"{new string(SplitChar, SplitNum)}{{0}}{new string(SplitChar, SplitNum)}";

        static readonly string NewLine = $"{Environment.NewLine}{Environment.NewLine}";

        internal static LogLevel LogLevel = LogLevel.Information;

        static bool EnableTraceLog = false;

        #endregion

        static FrameworkLogManager()
        {
            EnableTraceLog = SwitchManager.ShouldTraceFramework(sw =>
            {
                EnableTraceLog = SwitchManager.ShouldTraceFramework();
            });
        }

        internal static void Log(string categoryName, string message)
        {
            LogManager.Log(categoryName, LogLevel, $"{NewLine}{message}");
        }

        static string GetTitle(string title)
        {
            return string.Format(TitleLogTemplate, title);
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
        /// Log begin work
        /// </summary>
        /// <param name="work"></param>
        internal static void LogWorkBegin(IWork work, CommandExecutionOptions options = null)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【Work:{work.WorkId}】Start submitting work");
                string content = options == null ? string.Empty : JsonSerializer.Serialize(options);
                Log(string.Empty, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log begin work
        /// </summary>
        /// <param name="work"></param>
        internal static void LogWorkSuccessful(IWork work, WorkCommitResult commitResult)
        {
            if (EnableTraceLog && work != null && commitResult != null)
            {
                string title = GetTitle($"【{work.WorkId}】Work submit successful");
                string content = $"Total command count：{commitResult.CommittedCommandCount},Affected data count：{commitResult.AffectedDataCount},Allow empty result command count：{commitResult.AllowEmptyCommandCount}";
                Log(string.Empty, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log work failed
        /// </summary>
        /// <param name="work"></param>
        internal static void LogWorkFailed(IWork work, WorkCommitResult commitResult)
        {
            if (EnableTraceLog && work != null)
            {
                string title = GetTitle($"【{work.WorkId}】Work submit Failed");
                string content = $"Total command count：{commitResult.CommittedCommandCount},Affected data count：{commitResult.AffectedDataCount},Allow empty result command count：{commitResult.AllowEmptyCommandCount}";
                Log(string.Empty, GetMultilineMessage(title, content));
            }
        }

        /// <summary>
        /// Log begin exception
        /// </summary>
        /// <param name="work"></param>
        internal static void LogWorkException(IWork work, Exception ex)
        {
            if (EnableTraceLog && work != null && ex != null)
            {
                string title = GetTitle($"【{work.WorkId}】Work submit exception");
                Log(string.Empty, GetMultilineMessage(title, ex.Message));
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
                Log(string.Empty, message);
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
                Log(string.Empty, message);
            }
        }

        #endregion

        #region Database script

        /// <summary>
        /// Log database script
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        public static void LogDatabaseScript(DatabaseServerType databaseServerType, string script, object parameters)
        {
            if (EnableTraceLog && !string.IsNullOrWhiteSpace(script))
            {
                string title = GetTitle($"【{databaseServerType}】Execution script");
                //string content
                Log(string.Empty, GetMultilineMessage(GetTitle(title), script, JsonSerializer.Serialize(parameters)));
            }
        }

        /// <summary>
        /// Log database execution command
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="cmd">Command</param>
        public static void LogDatabaseExecutionCommand(DatabaseServerType databaseServerType, DatabaseExecutionCommand cmd)
        {
            if (EnableTraceLog && cmd != null)
            {
                LogDatabaseScript(databaseServerType, cmd.CommandText, cmd.Parameters);
            }
        }

        #endregion
    }
}
