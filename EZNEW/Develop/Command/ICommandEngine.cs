using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EZNEW.Develop.Entity;
using EZNEW.Paging;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command execute engine contract
    /// </summary>
    public interface ICommandEngine
    {
        #region Properties

        /// <summary>
        /// Gets the command engine identity key
        /// </summary>
        string IdentityKey { get; }

        #endregion

        #region Execute methods

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        int Execute(CommandExecuteOption executeOption, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        int Execute(CommandExecuteOption executeOption, params ICommand[] commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        Task<int> ExecuteAsync(CommandExecuteOption executeOption, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        Task<int> ExecuteAsync(CommandExecuteOption executeOption, params ICommand[] commands);

        #endregion

        #region Query methods

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        IEnumerable<T> Query<T>(ICommand command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        Task<IEnumerable<T>> QueryAsync<T>(ICommand command);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        IPaging<T> QueryPaging<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        Task<IPaging<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Determine whether does the data exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether does the data exist</returns>
        bool Query(ICommand command);

        /// <summary>
        /// Determine whether does the data exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether does the data exist</returns>
        Task<bool> QueryAsync(ICommand command);

        /// <summary>
        /// Gets aggregate data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        T AggregateValue<T>(ICommand command);

        /// <summary>
        /// Gets aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        Task<T> AggregateValueAsync<T>(ICommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return data set</returns>
        Task<DataSet> QueryMultipleAsync(ICommand command);

        #endregion
    }
}
