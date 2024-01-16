using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Development.Data.Command.Event;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command
    /// </summary>
    public class DataCommand
    {
        private DataCommand()
        {
            Id = CreateCommandId();
        }

        #region Properties

        /// <summary>
        /// Gets the command id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the command script
        /// </summary>
        public string Script { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the script parameters
        /// </summary>
        public DataCommandParameters ScriptParameters { get; private set; }

        /// <summary>
        /// Gets or sets the data script type
        /// </summary>
        public DataScriptType ScriptType { get; set; } = DataScriptType.Text;

        /// <summary>
        /// Gets or sets the result type
        /// </summary>
        public CommandResultType CommandResultType { get; set; } = CommandResultType.AffectedRows;

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the object key values
        /// </summary>
        public Dictionary<string, dynamic> EntityIdentityValues { get; private set; }

        /// <summary>
        /// Gets or sets the command properties
        /// </summary>
        public Dictionary<string, dynamic> Properties { get; set; } = null;

        /// <summary>
        /// Gets or sets execute mode
        /// </summary>
        public CommandExecutionMode ExecutionMode { get; set; } = CommandExecutionMode.Transform;

        /// <summary>
        /// Gets or sets the queryable object
        /// </summary>
        public ISixnetQueryable Queryable { get; set; } = null;

        /// <summary>
        /// Gets or sets the command operation type
        /// </summary>
        public DataOperationType OperationType { get; set; } = DataOperationType.Query;

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the fields assignment
        /// </summary>
        public FieldsAssignment FieldsAssignment { get; set; }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// Bulk insert data table
        /// </summary>
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Gets or sets the paging filter
        /// </summary>
        public PagingFilter PagingFilter { get; set; }

        /// <summary>
        /// Gets or sets the options
        /// </summary>
        public DataOperationOptions Options { get; set; }

        /// <summary>
        /// Starting event handlers
        /// </summary>
        readonly List<IDataCommandStartingEventHandler> StartingEventHandlers = new();

        /// <summary>
        /// Sync callback event handlers
        /// </summary>
        readonly List<IDataCommandCallbackEventHandler> CallbackEventHandlers = new();

        #endregion

        #region Methods

        #region Create

        /// <summary>
        /// Create a new command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="operationType">Operation type</param>
        /// <returns>Return a default command</returns>
        public static DataCommand Create<TEntity>(DataOperationType operationType)
        {
            var entityType = typeof(TEntity);
            var command = new DataCommand()
            {
                EntityType = entityType,
                OperationType = operationType,
                TableName = EntityManager.GetTableName(entityType)
            };
            return command;
        }

        /// <summary>
        /// Create a new ommand
        /// </summary>
        /// <param name="operationType">Operation type</param>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public static DataCommand Create(DataOperationType operationType, ISixnetQueryable queryable)
        {
            var entityType = queryable?.GetModelType();
            return new DataCommand()
            {
                OperationType = operationType,
                Queryable = queryable,
                EntityType = entityType,
                TableName = EntityManager.GetTableName(entityType)
            };
        }

        /// <summary>
        /// Create a new data command
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public static DataCommand CreateQueryCommand<TEntity>(ISixnetQueryable queryable)
        {
            var entityType = queryable?.GetModelType();
            if (entityType == null)
            {
                entityType = typeof(TEntity);
            }
            return new DataCommand()
            {
                OperationType = DataOperationType.Query,
                Queryable = queryable,
                EntityType = entityType,
                TableName = EntityManager.GetTableName(entityType)
            };
        }

        /// <summary>
        /// Create a new data command
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public static DataCommand CreateQueryCommand(ISixnetQueryable queryable)
        {
            var entityType = queryable?.GetModelType();
            return new DataCommand()
            {
                OperationType = DataOperationType.Query,
                Queryable = queryable,
                EntityType = entityType,
                TableName = EntityManager.GetTableName(entityType)
            };
        }

        /// <summary>
        /// Create a new data command
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <returns></returns>
        public static DataCommand Create(DataTable dataTable)
        {
            return new DataCommand()
            {
                OperationType = DataOperationType.BulkInsert,
                DataTable = dataTable
            };
        }

        /// <summary>
        /// Create script command
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <returns></returns>
        public static DataCommand CreateScriptCommand(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text)
        {
            return new DataCommand()
            {
                Script = script,
                ExecutionMode = CommandExecutionMode.Script,
                ScriptType = scriptType,
                ScriptParameters = DataCommandParameters.Parse(parameters)
            };
        }

        #endregion

        #region Command starting event

        #region Subscribe Command starting event

        /// <summary>
        /// Subscribe starting event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeStartingEvent(IDataCommandStartingEventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                StartingEventHandlers.Add(eventHandler);
            }
        }

        #endregion

        #region Trigger Command starting event

        /// <summary>
        /// Trigger Command starting event
        /// </summary>
        /// <returns>Return starting event result</returns>
        public void TriggerStartingEvent(DataCommandStartingEvent dataCommandStartingEvent)
        {
            if (!StartingEventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in StartingEventHandlers)
                {
                    handler.Handle(dataCommandStartingEvent);
                }
            }
        }

        #endregion

        #endregion

        #region Command callback event

        #region Subscribe callback event

        /// <summary>
        /// Subscribe callback event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeCallbackEvent(IDataCommandCallbackEventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                CallbackEventHandlers.Add(eventHandler);
            }
        }

        #endregion

        #region Trigger callback event

        /// <summary>
        /// Trigger callback event
        /// </summary>
        /// <returns>Return command executed event result</returns>
        public void TriggerCallbackEvent(DataCommandCallbackEvent dataCommandCallbackEvent)
        {
            CallbackEventHandlers?.ForEach(handler =>
            {
                handler.ExecuteAsync(dataCommandCallbackEvent);
            });
        }

        #endregion

        #endregion

        #region Clone

        /// <summary>
        /// Clone a ICommand object
        /// </summary>
        /// <returns></returns>
        public DataCommand Clone()
        {
            var newCommand = new DataCommand()
            {
                Id = Id,
                Script = Script,
                ScriptType = ScriptType,
                CommandResultType = CommandResultType,
                TableName = TableName,
                EntityIdentityValues = EntityIdentityValues?.ToDictionary(c => c.Key, c => c.Value),
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value),
                ExecutionMode = ExecutionMode,
                Queryable = Queryable?.Clone(),
                OperationType = OperationType,
                FieldsAssignment = FieldsAssignment?.Clone(),
                EntityType = EntityType,
                ScriptParameters = ScriptParameters?.Clone(),
                DataTable = DataTable
            };
            newCommand.StartingEventHandlers.AddRange(StartingEventHandlers);
            newCommand.CallbackEventHandlers.AddRange(CallbackEventHandlers);
            return newCommand;
        }

        #endregion

        #region Create command id

        /// <summary>
        /// Create a new command id
        /// </summary>
        /// <returns></returns>
        static string CreateCommandId()
        {
            return "C" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpper();
        }

        #endregion

        #region Parameters

        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public void SetParameters(DataCommandParameters parameters)
        {
            ScriptParameters = parameters;
            ResetIdentityValues();
        }

        /// <summary>
        /// Reset identity values
        /// </summary>
        void ResetIdentityValues()
        {
            EntityIdentityValues?.Clear();
            if (ScriptParameters != null && EntityType != null)
            {
                var primaryFields = EntityManager.GetPrimaryKeyNames(EntityType);
                if (primaryFields.IsNullOrEmpty())
                {
                    return;
                }
                EntityIdentityValues ??= new Dictionary<string, dynamic>();
                foreach (var field in primaryFields)
                {
                    if (ScriptParameters.Items.TryGetValue(field, out DataCommandParameterItem parameterItem))
                    {
                        EntityIdentityValues.Add(field, parameterItem?.Value);
                    }
                }
            }
        }

        #endregion

        #region Entity type

        /// <summary>
        /// Get entity type
        /// </summary>
        /// <returns></returns>
        public Type GetEntityType()
        {
            return EntityType ?? Queryable?.GetModelType();
        }

        /// <summary>
        /// Set entity type
        /// </summary>
        /// <param name="entityType">Entity type</param>
        public void SetEntityType(Type entityType)
        {
            EntityType = entityType;
        }

        #endregion

        #endregion
    }
}
