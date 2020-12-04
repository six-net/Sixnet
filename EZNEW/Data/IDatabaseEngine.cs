using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EZNEW.Develop.Command;

namespace EZNEW.Data
{
    /// <summary>
    /// Database execute engine contract
    /// </summary>
    public interface IDatabaseEngine
    {
        #region Execute

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        int Execute(DatabaseServer server, CommandExecuteOptions executeOption, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        int Execute(DatabaseServer server, CommandExecuteOptions executeOption, params ICommand[] commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        Task<int> ExecuteAsync(DatabaseServer server, CommandExecuteOptions executeOption, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        Task<int> ExecuteAsync(DatabaseServer server, CommandExecuteOptions executeOption, params ICommand[] commands);

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        IEnumerable<T> Query<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        Task<IEnumerable<T>> QueryAsync<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">databse server</param>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        IEnumerable<T> QueryPaging<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">databse server</param>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        Task<IEnumerable<T>> QueryPagingAsync<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Query datas offset the specified numbers
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <param name="offsetNum">Offset num</param>
        /// <param name="size">Query size</param>
        /// <returns>Return datas</returns>
        IEnumerable<T> QueryOffset<T>(DatabaseServer server, ICommand command, int offsetNum = 0, int size = int.MaxValue);

        /// <summary>
        /// Query datas offset the specified numbers
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <param name="offsetNum">Offset num</param>
        /// <param name="size">Query size</param>
        /// <returns>Return datas</returns>
        Task<IEnumerable<T>> QueryOffsetAsync<T>(DatabaseServer server, ICommand command, int offsetNum = 0, int size = int.MaxValue);

        /// <summary>
        /// Determine whether data has existed
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return whether data has existed</returns>
        bool Query(DatabaseServer server, ICommand command);

        /// <summary>
        /// Determine whether data has existed
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return whether data has existed</returns>
        Task<bool> QueryAsync(DatabaseServer server, ICommand command);

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        T AggregateValue<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        Task<T> AggregateValueAsync<T>(DatabaseServer server, ICommand command);

        /// <summary>
        /// Query multiple
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns>Return data set</returns>
        Task<DataSet> QueryMultipleAsync(DatabaseServer server, ICommand command);

        #endregion
    }
}
