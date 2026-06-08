// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database provider contract
    /// </summary>
    public partial interface ISixnetDatabaseProvider
    {
        #region Connection

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns>Database connection</returns>
        IDbConnection GetDbConnection(SixnetDatabaseServer server);

        /// <summary>
        /// Get db connection meta
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        SixnetDatabaseConnectionMeta GetDbConnectionMeta(IDbConnection connection);

        #endregion

        #region Execution

        /// <summary>
        /// Execute multiple command
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Affected data numbers</returns>
        int Execute(SixnetMultipleDatabaseCommand command);

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        List<T> Query<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        T QueryFirst<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(SixnetQueryMappingDatabaseCommand<TFirst, TSecond, TReturn> command);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn> command);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn> command);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> command);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> command);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> command);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        SixnetPagingInfo<T> QueryPaging<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns>Return whether has data</returns>
        bool Exists(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        int Count(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the data</returns>
        T Scalar<T>(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Return the dataset</returns>
        DataSet QueryMultiple(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(SixnetMultipleDatabaseCommand command);

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
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(SixnetMultipleDatabaseCommand command);

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
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(SixnetMultipleDatabaseCommand command);

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
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(SixnetMultipleDatabaseCommand command);

        #endregion

        #region Insert

        /// <summary>
        /// Insert data and return auto identities
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Added data identities,Key: command id, Value: identity value</returns>
        Dictionary<string, TIdentity> InsertAndReturnIdentity<TIdentity>(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="command">Database bulk insert command</param>
        void BulkInsert(SixnetBulkInsertDatabaseCommand command);

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="command">command</param>
        /// <returns></returns>
        void Migrate(SixnetMigrationDatabaseCommand command);

        #endregion

        #region Get databases

        /// <summary>
        /// Get databases
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        List<SixnetDatabase> GetDatabases(SixnetDatabaseCommand command);

        #endregion

        #region Get tables

        /// <summary>
        /// Get table
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        List<SixnetDataTable> GetTables(SixnetDatabaseCommand command);

        #endregion

        #region Get views

        /// <summary>
        /// Get views
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        List<SixnetView> GetViews(SixnetDatabaseCommand command);

        #endregion

        #region Get stored procedures

        /// <summary>
        /// Get stored procedures
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        List<SixnetStoredProcedure> GetStoredProcedures(SixnetDatabaseCommand command);

        #endregion

        #region Get columns

        /// <summary>
        /// Get columns
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        List<SixnetColumn> GetColumns(SixnetDatabaseCommand command);

        #endregion
    }
}
