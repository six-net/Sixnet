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
        /// Success Callback
        /// </summary>
        event ExecuteCommandCallback SuccessCallbackAsync;

        /// <summary>
        /// Failed Callback
        /// </summary>
        event ExecuteCommandCallback FailedCallbackAsync;

        /// <summary>
        /// Before Execute
        /// </summary>
        event BeforeExecute BeforeExecuteAsync;

        /// <summary>
        /// Before Execute Request
        /// </summary>
        BeforeExecuteRequest BeforeRequest { get; set; }

        /// <summary>
        /// callback request
        /// </summary>
        CommandCallbackRequest CallbackRequest { get; set; }

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

        /// <summary>
        /// execute complete
        /// </summary>
        /// <param name="success">success</param>
        void ExecuteComplete(bool success);

        /// <summary>
        /// execute complete
        /// </summary>
        /// <param name="success">success</param>
        Task ExecuteCompleteAsync(bool success);

        /// <summary>
        /// execute before
        /// </summary>
        /// <returns></returns>
        bool ExecuteBefore();

        /// <summary>
        /// execute before
        /// </summary>
        /// <returns></returns>
        Task<bool> ExecuteBeforeAsync();

        #endregion
    }
}
