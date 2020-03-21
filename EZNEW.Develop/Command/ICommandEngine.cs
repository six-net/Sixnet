using EZNEW.Develop.Entity;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Data;
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
        /// <param name="executeOption">execute option</param>
        /// <param name="cmds">commands</param>
        /// <returns>date numbers </returns>
        int Execute(CommandExecuteOption executeOption, params ICommand[] cmds);

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="executeOption">execute option</param>
        /// <param name="cmds">commands</param>
        /// <returns>date numbers </returns>
        Task<int> ExecuteAsync(CommandExecuteOption executeOption, params ICommand[] cmds);

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
        IPaging<T> QueryPaging<T>(ICommand cmd) where T : BaseEntity<T>, new();

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns></returns>
        Task<IPaging<T>> QueryPagingAsync<T>(ICommand cmd) where T : BaseEntity<T>, new();

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
        /// query aggregate data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        T QueryAggregateValue<T>(ICommand cmd);

        /// <summary>
        /// aggregate value
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>data</returns>
        Task<T> AggregateValueAsync<T>(ICommand cmd);

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="cmd">query cmd</param>
        /// <returns>data</returns>
        Task<DataSet> QueryMultipleAsync(ICommand cmd);

        #endregion
    }
}
