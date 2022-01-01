using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.UnitOfWork;

namespace EZNEW.Development.Command
{
    /// <summary>
    /// Defines default command
    /// </summary>
    public class DefaultCommand : ICommand
    {
        private DefaultCommand() { }

        #region Properties

        /// <summary>
        /// Gets the command id
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets or sets the command text
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public CommandParameters Parameters { get; set; } = null;

        /// <summary>
        /// Gets or sets the command text type
        /// </summary>
        public CommandTextType TextType { get; set; } = CommandTextType.Text;

        ///// <summary>
        ///// Indicates whether is transaction command
        ///// </summary>
        //public bool TransactionCommand { get; set; } = false;

        /// <summary>
        /// Gets or sets the result type
        /// </summary>
        public CommandResultType CommandResultType { get; set; } = CommandResultType.AffectedRows;

        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string EntityObjectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the object key values
        /// </summary>
        public Dictionary<string, dynamic> EntityIdentityValues { get; set; } = null;

        /// <summary>
        /// Gets or sets the command properties
        /// </summary>
        public Dictionary<string, dynamic> Properties { get; set; } = null;

        /// <summary>
        /// Gets or sets execute mode
        /// </summary>
        public CommandExecutionMode ExecutionMode { get; set; } = CommandExecutionMode.Transform;

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; } = null;

        /// <summary>
        /// Gets or sets the command operation type
        /// </summary>
        public CommandOperationType OperationType { get; set; } = CommandOperationType.Query;

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public IEnumerable<string> Fields { get; set; } = null;

        /// <summary>
        /// Indicates whether is obsolete
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
        /// Indicates whether must affect data
        /// </summary>
        public bool MustAffectedData { get; set; }

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
        /// Create a new command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="operationType">Operation type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="entityObjectName">Entity object name</param>
        /// <param name="entityIdentityValues">Entity identity values</param>
        /// <param name="properties">Properties</param>
        /// <returns>Return a default command</returns>
        public static DefaultCommand Create<TEntity>(CommandOperationType operationType, CommandParameters parameters = null, string entityObjectName = "", Dictionary<string, dynamic> entityIdentityValues = null, Dictionary<string, dynamic> properties = null)
        {
            return new DefaultCommand()
            {
                Id = WorkManager.Current?.GetCommandId() ?? DateTimeOffset.Now.Ticks,
                EntityType = typeof(TEntity),
                OperationType = operationType,
                Parameters = parameters,
                EntityObjectName = entityObjectName,
                EntityIdentityValues = entityIdentityValues,
                Properties = properties
            };
        }

        /// <summary>
        /// Create a new command
        /// </summary>
        /// <param name="operationType">Operation type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="entityObjectName">Entity object name</param>
        /// <param name="entityIdentityValues">Entity identity values</param>
        /// <param name="properties">Properties</param>
        /// <returns>Return a default command</returns>
        public static DefaultCommand Create(CommandOperationType operationType, CommandParameters parameters = null, string entityObjectName = "", Dictionary<string, dynamic> entityIdentityValues = null, Dictionary<string, dynamic> properties = null)
        {
            return new DefaultCommand()
            {
                Id = WorkManager.Current?.GetCommandId() ?? DateTimeOffset.Now.Ticks,
                OperationType = operationType,
                Parameters = parameters,
                EntityObjectName = entityObjectName,
                EntityIdentityValues = entityIdentityValues,
                Properties = properties
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
        public CommandStartingEventExecutionResult TriggerStartingEvent()
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
                return CommandStartingEventExecutionResult.DefaultSuccess;
            }
            CommandStartingEventExecutionResult result = null;
            foreach (var handler in SyncExecutingEventHandlers)
            {
                result = handler.Item1(handler.Item2);
                if (result?.BreakCommand ?? false)
                {
                    break;
                }
            }
            return result ?? CommandStartingEventExecutionResult.DefaultSuccess;

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

        #region Clone

        /// <summary>
        /// Clone a ICommand object
        /// </summary>
        /// <returns></returns>
        public ICommand Clone()
        {
            var newCommand = new DefaultCommand()
            {
                Id = Id,
                Text = Text,
                TextType = TextType,
                //TransactionCommand = TransactionCommand,
                CommandResultType = CommandResultType,
                EntityObjectName = EntityObjectName,
                EntityIdentityValues = EntityIdentityValues?.ToDictionary(c => c.Key, c => c.Value),
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value),
                ExecutionMode = ExecutionMode,
                Query = Query?.Clone(),
                OperationType = OperationType,
                Fields = Fields?.Select(c => c).ToList(),
                EntityType = EntityType,
                MustAffectedData = MustAffectedData,
                Parameters = Parameters?.Clone()
            };
            newCommand.SyncExecutingEventHandlers.AddRange(SyncExecutingEventHandlers);
            newCommand.AsyncExecutingEventHandlers.AddRange(AsyncExecutingEventHandlers);
            newCommand.ExecutedEventHandlers.AddRange(ExecutedEventHandlers);
            return newCommand;
        }

        #endregion

        #endregion
    }
}
