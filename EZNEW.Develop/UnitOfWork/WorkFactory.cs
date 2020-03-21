using EZNEW.Framework.Paging;
using EZNEW.Framework.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Command;
using EZNEW.Framework.Extension;
using System.Data;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// UnitOfWork manager
    /// </summary>
    public class WorkFactory
    {
        #region fields

        /// <summary>
        /// current unitofwork
        /// </summary>
        static AsyncLocal<IUnitOfWork> current = new AsyncLocal<IUnitOfWork>();

        /// <summary>
        /// create work event handler
        /// </summary>
        static event Action<IUnitOfWork> createWorkEventHandler;

        /// <summary>
        /// commit success event handler
        /// </summary>
        static event Action<IUnitOfWork, CommitResult, IEnumerable<ICommand>> workCommitSuccessEventHandler;

        #endregion

        #region propertys

        /// <summary>
        /// current IUnitOfWork Object
        /// </summary>
        public static IUnitOfWork Current
        {
            get
            {
                return current?.Value;
            }
            internal set
            {
                current.Value = value;
            }
        }

        #endregion

        #region methods

        #region event handler

        #region register event handler

        /// <summary>
        /// register create work event handler
        /// </summary>
        /// <param name="handlers">handlers</param>
        public static void RegisterCreateWorkEventHandler(params Action<IUnitOfWork>[] handlers)
        {
            if (handlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in handlers)
            {
                createWorkEventHandler += handler;
            }
        }

        /// <summary>
        /// register work commit success event handler
        /// </summary>
        /// <param name="handlers">handlers</param>
        public static void RegisterWorkCommitSuccessEventHandler(params Action<IUnitOfWork, CommitResult, IEnumerable<ICommand>>[] handlers)
        {
            if (handlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in handlers)
            {
                workCommitSuccessEventHandler += handler;
            }
        }

        #endregion

        #region invoke event handler

        /// <summary>
        /// invoke create work eventhandler
        /// </summary>
        internal static void InvokeCreateWorkEventHandler(IUnitOfWork unitOfWork)
        {
            createWorkEventHandler?.Invoke(unitOfWork);
        }

        /// <summary>
        /// invoke commit success event
        /// </summary>
        internal static void InvokeWorkCommitSuccessEventHandler(IUnitOfWork unitOfWork, CommitResult commitResult, IEnumerable<ICommand> commands)
        {
            workCommitSuccessEventHandler?.Invoke(unitOfWork, commitResult, commands);
        }

        #endregion

        #endregion

        #region register activation record

        /// <summary>
        /// register activation record
        /// </summary>
        /// <param name="records"></param>
        public static void RegisterActivationRecord(params IActivationRecord[] records)
        {
            Current?.AddActivation(records);
        }

        #endregion

        #region query data

        /// <summary>
        /// query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">query command</param>
        /// <returns>datas</returns>
        public static IEnumerable<T> Query<T>(ICommand cmd)
        {
            return QueryAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">query command</param>
        /// <returns>datas</returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(ICommand cmd)
        {
            if (cmd?.IsObsolete ?? true)
            {
                return new List<T>(0);
            }
            return await CommandExecuteManager.QueryAsync<T>(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// query datas with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>datas</returns>
        public static IPaging<T> QueryPaging<T>(ICommand cmd) where T : BaseEntity<T>, new()
        {
            return QueryPagingAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query datas with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>datas</returns>
        public static async Task<IPaging<T>> QueryPagingAsync<T>(ICommand cmd) where T : BaseEntity<T>, new()
        {
            if (cmd?.IsObsolete ?? true)
            {
                return Paging<T>.EmptyPaging();
            }
            return await CommandExecuteManager.QueryPagingAsync<T>(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>whether data is exist</returns>
        public static bool Query(ICommand cmd)
        {
            return QueryAsync(cmd).Result;
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>whether data is exist</returns>
        public static async Task<bool> QueryAsync(ICommand cmd)
        {
            if (cmd?.IsObsolete ?? true)
            {
                return false;
            }
            return await CommandExecuteManager.QueryAsync(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// query single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        public static T QuerySingle<T>(ICommand cmd)
        {
            return QuerySingleAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        public static async Task<T> QuerySingleAsync<T>(ICommand cmd)
        {
            if (cmd?.IsObsolete ?? true)
            {
                return default;
            }
            if (cmd.Query == null)
            {
                cmd.Query = QueryFactory.Create();
            }
            cmd.Query.QuerySize = 1;
            var datas = await CommandExecuteManager.QueryAsync<T>(cmd).ConfigureAwait(false);
            if (datas.IsNullOrEmpty())
            {
                return default;
            }
            return datas.FirstOrDefault();
        }

        /// <summary>
        /// query aggregate data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        public static T AggregateValue<T>(ICommand cmd)
        {
            return AggregateValueAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query aggregate data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        public static async Task<T> AggregateValueAsync<T>(ICommand cmd)
        {
            return await CommandExecuteManager.AggregateValueAsync<T>(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="cmd">execute command</param>
        /// <returns>data</returns>
        public static async Task<DataSet> QueryMultipleAsync(ICommand cmd)
        {
            return await CommandExecuteManager.QueryMultipleAsync(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="commandText">command text</param>
        /// <param name="parameters">parameters</param>
        /// <param name="commandTextType">command text type</param>
        /// <returns></returns>
        public static async Task<DataSet> QueryMultipleAsync(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text)
        {
            var rdbCmd = RdbCommand.CreateNewCommand(OperateType.Query, parameters);
            rdbCmd.CommandType = commandTextType;
            rdbCmd.CommandText = commandText;
            return await QueryMultipleAsync(rdbCmd).ConfigureAwait(false);
        }

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="cmd">execute command</param>
        /// <returns>data</returns>
        public static DataSet QueryMultiple(ICommand cmd)
        {
            return QueryMultipleAsync(cmd).Result;
        }

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="commandText">command text</param>
        /// <param name="parameters">parameters</param>
        /// <param name="commandTextType">command text type</param>
        /// <returns></returns>
        public static DataSet QueryMultiple(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text)
        {
            return QueryMultipleAsync(commandText, parameters, commandTextType).Result;
        }

        #endregion

        #region execute command

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="cmds">commands</param>
        /// <returns>execute result</returns>
        public static async Task<int> ExecuteAsync(params ICommand[] cmds)
        {
            return await ExecuteAsync(CommandExecuteOption.Default, cmds).ConfigureAwait(false);
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="executeOption">execute option</param>
        /// <param name="cmds">commands</param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(CommandExecuteOption executeOption, params ICommand[] cmds)
        {
            if (cmds.IsNullOrEmpty())
            {
                return await Task.FromResult(0).ConfigureAwait(false);
            }
            return await CommandExecuteManager.ExecuteAsync(executeOption ?? CommandExecuteOption.Default, cmds).ConfigureAwait(false);
        }

        /// <summary>
        /// execute command text
        /// </summary>
        /// <param name="commandText">command text</param>
        /// <param name="parameters">parameters</param>
        /// <param name="commandTextType">command text type</param>
        /// <param name="executeOption">execute option</param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text, CommandExecuteOption executeOption = null)
        {
            var rdbCmd = RdbCommand.CreateNewCommand(OperateType.Query, parameters);
            rdbCmd.CommandType = commandTextType;
            rdbCmd.CommandText = commandText;
            rdbCmd.ExecuteMode = CommandExecuteMode.CommandText;
            return await ExecuteAsync(executeOption, rdbCmd).ConfigureAwait(false);
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="cmds">commands</param>
        /// <returns>execute result</returns>
        public static int Execute(params ICommand[] cmds)
        {
            return ExecuteAsync(cmds).Result;
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="executeOption">execute option</param>
        /// <param name="cmds">commands</param>
        /// <returns></returns>
        public static int Execute(CommandExecuteOption executeOption, params ICommand[] cmds)
        {
            return ExecuteAsync(executeOption, cmds).Result;
        }

        /// <summary>
        /// execute command text
        /// </summary>
        /// <param name="commandText">command text</param>
        /// <param name="parameters">parameters</param>
        /// <param name="commandTextType">command text type</param>
        /// <param name="executeOption">execute option</param>
        /// <returns></returns>
        public static int Execute(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text, CommandExecuteOption executeOption = null)
        {
            return ExecuteAsync(commandText, parameters, commandTextType, executeOption).Result;
        }

        #endregion

        #region create unitofwork

        /// <summary>
        /// create a new IUnitOrWork
        /// </summary>
        /// <param name="isolationLevel">data isolation level</param>
        /// <returns></returns>
        public static IUnitOfWork Create(DataIsolationLevel? isolationLevel = null)
        {
            return new DefaultUnitOfWork()
            {
                IsolationLevel = isolationLevel
            };
        }

        #endregion

        #endregion
    }
}
