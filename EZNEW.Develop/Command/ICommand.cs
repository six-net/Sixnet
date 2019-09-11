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
        #region Propertys

        /// <summary>
        /// command Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Command Text
        /// </summary>
        string CommandText { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        dynamic Parameters { get; set; }

        /// <summary>
        /// ObjectName
        /// </summary>
        string ObjectName { get; set; }

        /// <summary>
        /// ObjectKeys
        /// </summary>
        List<string> ObjectKeys { get; set; }

        /// <summary>
        /// ObjectKeyValues
        /// </summary>
        Dictionary<string, dynamic> ObjectKeyValues { get; set; }

        /// <summary>
        /// ServerKeys
        /// </summary>
        List<string> ServerKeys { get; set; }

        /// <summary>
        /// ServerKey Values
        /// </summary>
        Dictionary<string, dynamic> ServerKeyValues { get; set; }

        /// <summary>
        /// Execute Mode
        /// </summary>
        CommandExecuteMode ExecuteMode { get; set; }

        /// <summary>
        /// Query
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// Operate
        /// </summary>
        OperateType Operate { get; set; }

        /// <summary>
        /// Fields
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

        #region Methods

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
