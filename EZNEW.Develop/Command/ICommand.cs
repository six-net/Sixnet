using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command interface
    /// </summary>
    public interface ICommand
    {
        #region propertys

        /// <summary>
        /// command Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// command text
        /// </summary>
        string CommandText { get; set; }

        /// <summary>
        /// parameters
        /// </summary>
        dynamic Parameters { get; set; }

        /// <summary>
        /// object name
        /// </summary>
        string ObjectName { get; set; }

        /// <summary>
        /// object keys
        /// </summary>
        List<string> ObjectKeys { get; set; }

        /// <summary>
        /// object key values
        /// </summary>
        Dictionary<string, dynamic> ObjectKeyValues { get; set; }

        /// <summary>
        /// server keys
        /// </summary>
        List<string> ServerKeys { get; set; }

        /// <summary>
        /// server key values
        /// </summary>
        Dictionary<string, dynamic> ServerKeyValues { get; set; }

        /// <summary>
        /// execute mode
        /// </summary>
        CommandExecuteMode ExecuteMode { get; set; }

        /// <summary>
        /// query
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// operate
        /// </summary>
        OperateType Operate { get; set; }

        /// <summary>
        /// fields
        /// </summary>
        List<string> Fields { get; set; }

        /// <summary>
        /// direct return if command is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// must return value on success
        /// </summary>
        bool MustReturnValueOnSuccess { get; set; }

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
        void RegisterBeforeExecuteOperation(CommandBeforeExecuteOperation operation, CommandBeforeExecuteParameter beforeExecuteParameter, bool async = false);

        #endregion

        #region execute before operation

        /// <summary>
        /// execute before execute operation
        /// </summary>
        /// <returns></returns>
        CommandBeforeExecuteResult ExecuteBeforeExecuteOperation();

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
        void RegisterCallbackOperation(CommandCallbackOperation operation, CommandCallbackParameter parameter, bool async = true);

        #endregion

        #region execute callback operation

        /// <summary>
        /// execute callback operation
        /// </summary>
        /// <param name="success">command execute success</param>
        /// <returns></returns>
        CommandCallbackResult ExecuteCallbackOperation(bool success);

        #endregion

        #endregion

        #endregion
    }
}
