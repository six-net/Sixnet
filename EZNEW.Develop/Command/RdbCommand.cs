using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
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
        /// verify result method
        /// </summary>
        public Func<int, bool> VerifyResult { get; set; }

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
        /// success callback
        /// </summary>
        public event ExecuteCommandCallback SuccessCallbackAsync;

        /// <summary>
        /// failed callback
        /// </summary>
        public event ExecuteCommandCallback FailedCallbackAsync;

        /// <summary>
        /// before execute
        /// </summary>
        public event BeforeExecute BeforeExecuteAsync;

        /// <summary>
        /// callback request
        /// </summary>
        public CommandCallbackRequest CallbackRequest { get; set; }

        /// <summary>
        /// before request
        /// </summary>
        public BeforeExecuteRequest BeforeRequest { get; set; }

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
                Id = WorkFactory.Current?.GetRecordId() ?? DateTime.Now.Ticks,
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

        /// <summary>
        /// execute commplete
        /// </summary>
        /// <param name="success">success</param>
        public void ExecuteComplete(bool success)
        {
            ExecuteCompleteAsync(success).Wait();
        }

        /// <summary>
        /// execute commplete
        /// </summary>
        /// <param name="success">success</param>
        public async Task ExecuteCompleteAsync(bool success)
        {
            if (success)
            {
                if (SuccessCallbackAsync != null)
                {
                    await SuccessCallbackAsync(CallbackRequest).ConfigureAwait(false);
                }
            }
            else
            {
                if (FailedCallbackAsync != null)
                {
                    await FailedCallbackAsync(CallbackRequest).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// execute before
        /// </summary>
        /// <returns></returns>
        public bool ExecuteBefore()
        {
            return ExecuteBeforeAsync().Result;
        }

        /// <summary>
        /// execute before
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteBeforeAsync()
        {
            if (BeforeExecuteAsync != null)
            {
                return await BeforeExecuteAsync(BeforeRequest).ConfigureAwait(false);
            }
            return true;
        }

        #endregion
    }
}
