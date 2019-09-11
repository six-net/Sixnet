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

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// UnitOfWork Manager
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
        public static IPaging<T> QueryPaging<T>(ICommand cmd) where T : BaseEntity<T>
        {
            return QueryPagingAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query datas with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>datas</returns>
        public static async Task<IPaging<T>> QueryPagingAsync<T>(ICommand cmd) where T : BaseEntity<T>
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
                return default(T);
            }
            return await CommandExecuteManager.QuerySingleAsync<T>(cmd).ConfigureAwait(false);
        }

        #endregion

        #region create unitofwork

        /// <summary>
        /// create a new IUnitOrWork
        /// </summary>
        /// <returns></returns>
        public static IUnitOfWork Create()
        {
            return new DefaultUnitOfWork();
        }

        #endregion

        #endregion
    }
}
