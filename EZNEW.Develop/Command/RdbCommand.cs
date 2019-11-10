using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Internal.MQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command for RDB
    /// </summary>
    public class RdbCommand : ICommand
    {
        private RdbCommand()
        { }

        #region propertys

        /// <summary>
        /// command id
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// command text
        /// </summary>
        public string CommandText { get; set; } = string.Empty;

        /// <summary>
        /// parameters
        /// </summary>
        public object Parameters { get; set; } = null;

        /// <summary>
        /// command type
        /// </summary>
        public RdbCommandTextType CommandType { get; set; } = RdbCommandTextType.Text;

        /// <summary>
        /// transaction command
        /// </summary>
        public bool TransactionCommand { get; set; } = false;

        /// <summary>
        /// result type
        /// </summary>
        public ExecuteCommandResult CommandResultType { get; set; } = ExecuteCommandResult.ExecuteRows;

        /// <summary>
        /// object name
        /// </summary>
        public string ObjectName { get; set; } = string.Empty;

        /// <summary>
        /// object keys
        /// </summary>
        public List<string> ObjectKeys { get; set; } = null;

        /// <summary>
        /// object key values
        /// </summary>
        public Dictionary<string, dynamic> ObjectKeyValues { get; set; } = null;

        /// <summary>
        /// server keys
        /// </summary>
        public List<string> ServerKeys { get; set; } = null;

        /// <summary>
        /// server key values
        /// </summary>
        public Dictionary<string, dynamic> ServerKeyValues { get; set; } = null;

        /// <summary>
        /// execute mode
        /// </summary>
        public CommandExecuteMode ExecuteMode { get; set; } = CommandExecuteMode.Transform;

        /// <summary>
        /// query object
        /// </summary>
        public IQuery Query { get; set; } = null;

        /// <summary>
        /// operate
        /// </summary>
        public OperateType Operate { get; set; } = OperateType.Query;

        /// <summary>
        /// fields
        /// </summary>
        public List<string> Fields { get; set; } = null;

        /// <summary>
        /// direct return if command is obsolete
        /// </summary>
        public bool IsObsolete
        {
            get
            {
                return Query?.IsObsolete ?? false;
            }
        }

        /// <summary>
        /// entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// must return value on success
        /// </summary>
        public bool MustReturnValueOnSuccess
        {
            get; set;
        }

        /// <summary>
        /// sync before operations
        /// </summary>
        List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>> SyncBeforeOperations = new List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>>();

        /// <summary>
        /// async before operations
        /// </summary>
        List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>> AsyncBeforeOperations = new List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>>();

        /// <summary>
        /// sync callback
        /// </summary>
        List<Tuple<CommandCallbackOperation, CommandCallbackParameter>> SyncCallbackOperations = new List<Tuple<CommandCallbackOperation, CommandCallbackParameter>>();

        /// <summary>
        /// async callback
        /// </summary>
        List<Tuple<CommandCallbackOperation, CommandCallbackParameter>> AsyncCallbackOperations = new List<Tuple<CommandCallbackOperation, CommandCallbackParameter>>();

        #endregion

        #region static methods

        /// <summary>
        /// get a new rdbcommand object
        /// </summary>
        /// <param name="operate">operate</param>
        /// <param name="parameters">parameters</param>
        /// <param name="objectName">objectName</param>
        /// <param name="objectKey">objectKey</param>
        /// <returns></returns>
        public static RdbCommand CreateNewCommand<T>(OperateType operate, object parameters = null, string objectName = "", List<string> objectKeys = null, Dictionary<string, dynamic> objectKeyValues = null, List<string> serverKeys = null, Dictionary<string, dynamic> serverKeyValues = null)
        {
            return new RdbCommand()
            {
                Id = WorkFactory.Current?.GetCommandId() ?? DateTime.Now.Ticks,
                EntityType = typeof(T),
                Operate = operate,
                Parameters = parameters,
                ObjectName = objectName,
                ObjectKeyValues = objectKeyValues,
                ServerKeyValues = serverKeyValues,
                ObjectKeys = objectKeys,
                ServerKeys = serverKeys
            };
        }

        #endregion

        #region methods

        #region command execute before operation

        #region register before execute sync operation

        /// <summary>
        /// register before execute sync operation
        /// </summary>
        /// <param name="operation">operation</param>
        /// <param name="beforeExecuteParameter">parameter</param>
        /// <param name="async">execute by async</param>
        public void RegisterBeforeExecuteOperation(CommandBeforeExecuteOperation operation, CommandBeforeExecuteParameter beforeExecuteParameter, bool async = false)
        {
            if (operation == null)
            {
                return;
            }
            if (async)
            {
                AsyncBeforeOperations.Add(new Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>(operation, beforeExecuteParameter));
            }
            else
            {
                SyncBeforeOperations.Add(new Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>(operation, beforeExecuteParameter));
            }
        }

        #endregion

        #region execute before operation

        /// <summary>
        /// execute before execute operation
        /// </summary>
        /// <returns></returns>
        public CommandBeforeExecuteResult ExecuteBeforeExecuteOperation()
        {
            #region execute async operation

            if (!AsyncBeforeOperations.IsNullOrEmpty())
            {
                var internalMsgItem = new CommandBeforeOperationInternalMessageCommand(AsyncBeforeOperations);
                InternalMessageQueue.Enqueue(internalMsgItem);
            }

            #endregion

            #region execut sync operation

            if (SyncBeforeOperations.IsNullOrEmpty())
            {
                return CommandBeforeExecuteResult.DefaultSuccess; ;
            }
            CommandBeforeExecuteResult result = new CommandBeforeExecuteResult();
            SyncBeforeOperations.ForEach(op =>
                        {
                            var opResult = op.Item1(op.Item2);
                            result.AllowExecuteCommand = result.AllowExecuteCommand && opResult.AllowExecuteCommand;
                        });
            return result;

            #endregion
        }

        #endregion

        #endregion

        #region command callback operation

        #region register command callback operation

        /// <summary>
        /// register command callback operation
        /// </summary>
        /// <param name="operation">operation</param>
        /// <param name="parameter">parameter</param>
        /// <param name="async">execute callback by async</param>
        public void RegisterCallbackOperation(CommandCallbackOperation operation, CommandCallbackParameter parameter, bool async = true)
        {
            if (operation == null)
            {
                return;
            }
            if (async)
            {
                AsyncCallbackOperations.Add(new Tuple<CommandCallbackOperation, CommandCallbackParameter>(operation, parameter));
            }
            else
            {
                SyncCallbackOperations.Add(new Tuple<CommandCallbackOperation, CommandCallbackParameter>(operation, parameter));
            }
        }

        #endregion

        #region execute callback operation

        /// <summary>
        /// execute callback operation
        /// </summary>
        /// <param name="success">command execute success</param>
        /// <returns></returns>
        public CommandCallbackResult ExecuteCallbackOperation(bool success)
        {
            #region execute async operation

            if (!AsyncCallbackOperations.IsNullOrEmpty())
            {
                var internalMsgItem = new CommandCallbackInternalMessageCommand(AsyncCallbackOperations);
                InternalMessageQueue.Enqueue(internalMsgItem);
            }

            #endregion

            #region execute sync operation

            if (!SyncCallbackOperations.IsNullOrEmpty())
            {
                SyncCallbackOperations.ForEach(op =>
                {
                    if (op.Item2 != null)
                    {
                        op.Item2.ExecuteSuccess = success;
                    }
                    op.Item1(op.Item2);
                });
            }
            return CommandCallbackResult.Default;

            #endregion
        }

        #endregion

        #endregion

        #endregion
    }
}
