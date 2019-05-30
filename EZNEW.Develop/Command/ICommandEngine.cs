using EZNEW.Develop.Entity;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command execute engine
    /// </summary>
    public interface ICommandEngine
    {
        #region propertys

        /// <summary>
        /// command engine identity key
        /// </summary>
        string IdentityKey
        {
            get;
        }

        #endregion

        #region execute methods

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="cmds">commands</param>
        /// <returns>date numbers </returns>
        int Execute(params ICommand[] cmds);

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="cmds">commands</param>
        /// <returns>date numbers </returns>
        Task<int> ExecuteAsync(params ICommand[] cmds);

        #endregion

        #region query methods

        /// <summary>
        /// execute query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>datas</returns>
        IEnumerable<T> Query<T>(ICommand cmd);

        /// <summary>
        /// execute query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>datas</returns>
        Task<IEnumerable<T>> QueryAsync<T>(ICommand cmd);

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns></returns>
        IPaging<T> QueryPaging<T>(ICommand cmd) where T:BaseEntity<T>;

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns></returns>
        Task<IPaging<T>> QueryPagingAsync<T>(ICommand cmd) where T : BaseEntity<T>;

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>data is exist</returns>
        bool Query(ICommand cmd);

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>data is exist</returns>
        Task<bool> QueryAsync(ICommand cmd);

        /// <summary>
        /// query a single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        T QuerySingle<T>(ICommand cmd);

        /// <summary>
        /// query a single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        Task<T> QuerySingleAsync<T>(ICommand cmd);

        #endregion
    }
}
