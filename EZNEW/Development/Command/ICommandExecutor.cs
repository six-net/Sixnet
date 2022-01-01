﻿using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EZNEW.Data;
using EZNEW.Development.Entity;
using EZNEW.Paging;

namespace EZNEW.Development.Command
{
    /// <summary>
    /// Defines command executor contract
    /// </summary>
    public interface ICommandExecutor
    {
        #region Properties

        /// <summary>
        /// Gets the command executor identity key
        /// </summary>
        string IdentityValue { get; }

        #endregion

        #region Execute methods

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect numbers</returns>
        int Execute(CommandExecutionOptions executionOptions, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        int Execute(CommandExecutionOptions executionOptions, params ICommand[] commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        Task<int> ExecuteAsync(CommandExecutionOptions executionOptions, IEnumerable<ICommand> commands);

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effect numbers</returns>
        Task<int> ExecuteAsync(CommandExecutionOptions executionOptions, params ICommand[] commands);

        #endregion

        #region Query methods

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        IEnumerable<T> Query<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        Task<IEnumerable<T>> QueryAsync<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        PagingInfo<T> QueryPaging<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether does the data exist</returns>
        bool Exists(ICommand command);

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether exists data</returns>
        Task<bool> ExistsAsync(ICommand command);

        /// <summary>
        /// Gets aggregation data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return value</returns>
        T AggregateValue<T>(ICommand command);

        /// <summary>
        /// Gets aggregation value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return value</returns>
        Task<T> AggregateValueAsync<T>(ICommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return data set</returns>
        Task<DataSet> QueryMultipleAsync(ICommand command);

        #endregion

        #region Bulk insert

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        Task BulkInsertAsync(DatabaseServer server, DataTable dataTable, IBulkInsertionOptions bulkInsertOptions = null);

        #endregion
    }
}
