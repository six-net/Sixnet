// "Company © 2025. All rights reserved."

using System.Data;
using System.Threading.Tasks;

using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database provider contract
    /// </summary>
    public partial interface ISixnetDatabaseProvider
    {
        #region Execution

        /// <summary>
        /// Execute multiple command
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Affected data numbers</returns>
        Task<int> ExecuteAsync(SixnetMultipleDatabaseCommand command);

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        Task<List<T>> QueryAsync<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        Task<T> QueryFirstAsync<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(SixnetQueryMappingDatabaseCommand<TFirst, TSecond, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> command);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        Task<SixnetPagingInfo<T>> QueryPagingAsync<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns>Whether has data</returns>
        Task<bool> ExistsAsync(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<int> CountAsync(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the data</returns>
        Task<T> ScalarAsync<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Return the dataset</returns>
        Task<DataSet> QueryMultipleAsync(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(SixnetMultipleDatabaseCommand command);

        #endregion

        #region Insert

        /// <summary>
        /// Insert data and return identities
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Added data identities,Key: command id, Value: identity value</returns>
        Task<Dictionary<string, TIdentity>> InsertAndReturnIdentityAsync<TIdentity>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="command">Database bulk insert command</param>
        Task BulkInsertAsync(SixnetBulkInsertDatabaseCommand command);

        #endregion

        #region Migration

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="command">command</param>
        /// <returns></returns>
        Task MigrateAsync(SixnetMigrationDatabaseCommand command);

        #endregion

        #region Get databases

        /// <summary>
        /// Get databases
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        Task<List<SixnetDatabase>> GetDatabasesAsync(SixnetDatabaseCommand command);

        #endregion

        #region Get tables

        /// <summary>
        /// Get table
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        Task<List<SixnetDataTable>> GetTablesAsync(SixnetDatabaseCommand command);

        #endregion

        #region Get views

        /// <summary>
        /// Get views
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SixnetView>> GetViewsAsync(SixnetDatabaseCommand command);

        #endregion

        #region Get stored procedures

        /// <summary>
        /// Get stored procedures
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SixnetStoredProcedure>> GetStoredProceduresAsync(SixnetDatabaseCommand command);

        #endregion

        #region Get columns

        /// <summary>
        /// Get columns
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SixnetColumn>> GetColumnsAsync(SixnetDatabaseCommand command);

        #endregion
    }
}
