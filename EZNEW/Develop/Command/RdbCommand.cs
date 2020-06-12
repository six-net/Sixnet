using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command for rdb
    /// </summary>
    public class RdbCommand : ICommand
    {
        private RdbCommand() { }

        #region Properties

        /// <summary>
        /// Gets the command id
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets or sets the command text
        /// </summary>
        public string CommandText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public object Parameters { get; set; } = null;

        /// <summary>
        /// Gets or sets the command type
        /// </summary>
        public CommandTextType CommandType { get; set; } = CommandTextType.Text;

        /// <summary>
        /// Gets or sets whether is transaction command
        /// </summary>
        public bool TransactionCommand { get; set; } = false;

        /// <summary>
        /// Gets or sets the result type
        /// </summary>
        public ExecuteCommandResult CommandResultType { get; set; } = ExecuteCommandResult.ExecuteRows;

        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string ObjectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the object keys
        /// </summary>
        public List<string> ObjectKeys { get; set; } = null;

        /// <summary>
        /// Gets or sets the object key values
        /// </summary>
        public Dictionary<string, dynamic> ObjectKeyValues { get; set; } = null;

        /// <summary>
        /// Gets or sets the server keys
        /// </summary>
        public List<string> ServerKeys { get; set; } = null;

        /// <summary>
        /// Gets or sets the server key values
        /// </summary>
        public Dictionary<string, dynamic> ServerKeyValues { get; set; } = null;

        /// <summary>
        /// Gets or sets execute mode
        /// </summary>
        public CommandExecuteMode ExecuteMode { get; set; } = CommandExecuteMode.Transform;

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; } = null;

        /// <summary>
        /// Gets or sets the command operate type
        /// </summary>
        public OperateType OperateType { get; set; } = OperateType.Query;

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public List<string> Fields { get; set; } = null;

        /// <summary>
        /// Gets if the command is obsolete
        /// </summary>
        public bool IsObsolete
        {
            get
            {
                return Query?.IsObsolete ?? false;
            }
        }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets whether must return value on success
        /// </summary>
        public bool MustReturnValueOnSuccess
        {
            get; set;
        }

        /// <summary>
        /// Sync executing event handlers
        /// </summary>
        readonly List<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>> SyncExecutingEventHandlers = new List<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>>();

        /// <summary>
        /// Async executing event handlers
        /// </summary>
        readonly List<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>> AsyncExecutingEventHandlers = new List<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>>();

        /// <summary>
        /// Sync executed event handlers
        /// </summary>
        readonly List<Tuple<CommandCallbackEventHandler, CommandCallbackEventParameter>> ExecutedEventHandlers = new List<Tuple<CommandCallbackEventHandler, CommandCallbackEventParameter>>();

        #endregion

        #region Static methods

        /// <summary>
        /// Create a new rdbcommand object
        /// </summary>
        /// <param name="operateType">Command operate type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="objectName">Object name</param>
        /// <param name="objectKey">Object key</param>
        /// <returns>Return a rdb command</returns>
        public static RdbCommand CreateNewCommand<T>(OperateType operateType, object parameters = null, string objectName = "", List<string> objectKeys = null, Dictionary<string, dynamic> objectKeyValues = null, List<string> serverKeys = null, Dictionary<string, dynamic> serverKeyValues = null)
        {
            return new RdbCommand()
            {
                Id = WorkManager.Current?.GetCommandId() ?? DateTimeOffset.Now.Ticks,
                EntityType = typeof(T),
                OperateType = operateType,
                Parameters = parameters,
                ObjectName = objectName,
                ObjectKeyValues = objectKeyValues,
                ServerKeyValues = serverKeyValues,
                ObjectKeys = objectKeys,
                ServerKeys = serverKeys
            };
        }

        /// <summary>
        /// Create a new rdbcommand object
        /// </summary>
        /// <param name="operateType">Command operate type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="objectName">Object name</param>
        /// <param name="objectKey">Object key</param>
        /// <returns>Return a rdb command</returns>
        public static RdbCommand CreateNewCommand(OperateType operateType, object parameters = null, string objectName = "", List<string> objectKeys = null, Dictionary<string, dynamic> objectKeyValues = null, List<string> serverKeys = null, Dictionary<string, dynamic> serverKeyValues = null)
        {
            return new RdbCommand()
            {
                Id = WorkManager.Current?.GetCommandId() ?? DateTimeOffset.Now.Ticks,
                OperateType = operateType,
                Parameters = parameters,
                ObjectName = objectName,
                ObjectKeyValues = objectKeyValues,
                ServerKeyValues = serverKeyValues,
                ObjectKeys = objectKeys,
                ServerKeys = serverKeys
            };
        }

        #endregion

        #region Methods

        #region Command starting event

        #region Listen Command starting event

        /// <summary>
        /// Listen Command starting event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="startingEventParameter">Parameter</param>
        /// <param name="async">Whether execute event handler by async</param>
        public void ListenStarting(CommandStartingEventHandler eventHandler, CommandStartingEventParameter startingEventParameter, bool async = false)
        {
            if (eventHandler == null)
            {
                return;
            }
            if (async)
            {
                AsyncExecutingEventHandlers.Add(new Tuple<CommandStartingEventHandler, CommandStartingEventParameter>(eventHandler, startingEventParameter));
            }
            else
            {
                SyncExecutingEventHandlers.Add(new Tuple<CommandStartingEventHandler, CommandStartingEventParameter>(eventHandler, startingEventParameter));
            }
        }

        #endregion

        #region Trigger Command starting event

        /// <summary>
        /// Trigger Command starting event
        /// </summary>
        /// <returns>Return starting event result</returns>
        public CommandStartingEventExecuteResult TriggerStartingEvent()
        {
            #region Execute async handlers

            if (!AsyncExecutingEventHandlers.IsNullOrEmpty())
            {
                AsyncExecutingEventHandlers.ForEach(handler =>
                {
                    var eventHandler = handler;
                    ThreadPool.QueueUserWorkItem(s => { eventHandler.Item1(handler.Item2); });
                });
            }

            #endregion

            #region Execut sync handlers

            if (SyncExecutingEventHandlers.IsNullOrEmpty())
            {
                return CommandStartingEventExecuteResult.DefaultSuccess; ;
            }
            var result = new CommandStartingEventExecuteResult();
            foreach (var handler in SyncExecutingEventHandlers)
            {
                var eventResult = handler.Item1(handler.Item2);
                result.AllowExecuteCommand = result.AllowExecuteCommand && eventResult.AllowExecuteCommand;
                if (!result.AllowExecuteCommand)
                {
                    break;
                }
            }
            return result;

            #endregion
        }

        #endregion

        #endregion

        #region Command callback event

        #region Listen command callback event

        /// <summary>
        /// Listen command callback event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="parameter">Parameter</param>
        public void ListenCallback(CommandCallbackEventHandler eventHandler, CommandCallbackEventParameter parameter)
        {
            if (eventHandler == null)
            {
                return;
            }
            ExecutedEventHandlers.Add(new Tuple<CommandCallbackEventHandler, CommandCallbackEventParameter>(eventHandler, parameter));
        }

        #endregion

        #region Trigger executed event

        /// <summary>
        /// Trigger callback event
        /// </summary>
        /// <param name="success">Whether command execute success</param>
        /// <returns>Return command executed event result</returns>
        public void TriggerCallbackEvent(bool success)
        {
            if (ExecutedEventHandlers.IsNullOrEmpty())
            {
                return;
            }
            ExecutedEventHandlers.ForEach(handler =>
            {
                var callbackHandler = handler;
                if (callbackHandler.Item2 != null)
                {
                    callbackHandler.Item2.IsExecuteSuccess = success;
                }
                ThreadPool.QueueUserWorkItem(s => { callbackHandler.Item1(handler.Item2); });
            });
        }

        #endregion

        #endregion

        #endregion
    }
}
