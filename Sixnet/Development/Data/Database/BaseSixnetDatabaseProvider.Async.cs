using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Sixnet.Development.Data.Dapper;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;
using System.Threading.Tasks;
using System.Linq;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines base database provider
    /// </summary>
    public abstract partial class BaseSixnetDatabaseProvider
    {
        #region Execution

        /// <summary>
        /// Execute multiple command
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Affected data numbers</returns>
        public virtual async Task<int> ExecuteAsync(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseExecutionStatements(command);
            var totalAffectedNumber = 0;
            foreach (var statement in statements)
            {
                totalAffectedNumber += await ExecuteDatabaseStatementAsync(command, statement).ConfigureAwait(false);
            }
            return totalAffectedNumber;
        }

        /// <summary>
        /// Execute database statement
        /// </summary>
        /// <param name="command">Database command</param>
        /// <param name="statement">Database execution statement</param>
        /// <returns></returns>
        async Task<int> ExecuteDatabaseStatementAsync(SixnetDatabaseCommand command, ExecutionDatabaseStatement statement)
        {
            return await command.Connection.DbConnection.ExecuteAsync(GetCommandDefinition(command, statement)).ConfigureAwait(false);
        }

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual async Task<List<T>> QueryAsync<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync<T>(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))?.ToList() ?? new List<T>(0);
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual async Task<T> QueryFirstAsync<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return await command.Connection.DbConnection.QueryFirstOrDefaultAsync<T>(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(QueryMappingDatabaseCommand<TFirst, TSecond, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

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
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

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
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

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
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

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
        public virtual async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return (await command.Connection.DbConnection.QueryAsync(GetCommandDefinition(command, queryStatement), command.DataMappingFunc, command.SpiltOnFieldName).ConfigureAwait(false))?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual async Task<PagingInfo<T>> QueryPagingAsync<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryPagingStatement(command);
            var pagingDatas = (await command.Connection.DbConnection.QueryAsync<PagingTotalCountModel, T, PagingTotalCountMappingModel<T>>(GetCommandDefinition(command, queryStatement)
                               , PagingTotalCountMappingModel<T>.PagingTotalCountMappingFunc, SixnetDataManager.PagingTotalCountSplitFieldName).ConfigureAwait(false)
                               )?.ToList() ?? new List<PagingTotalCountMappingModel<T>>(0);
            if (pagingDatas.IsNullOrEmpty())
            {
                return SixnetPager.Empty<T>();
            }
            var firstData = pagingDatas.First();
            var pagingFilter = command.DataCommand.PagingFilter;
            return SixnetPager.Create(pagingFilter.Page, pagingFilter.PageSize, firstData.PagingTotalDataCount, pagingDatas.Select(c => c.RealReturnData));
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns>Whether has data</returns>
        public virtual async Task<bool> ExistsAsync(SingleDatabaseCommand command)
        {
            command?.DataCommand?.Queryable?.Output(QueryableOutputType.Predicate);
            return (await ScalarAsync<int>(command).ConfigureAwait(false)) > 0;
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(SingleDatabaseCommand command)
        {
            command?.DataCommand?.Queryable?.Output(QueryableOutputType.Count);
            return await ScalarAsync<int>(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the data</returns>
        public virtual async Task<T> ScalarAsync<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return await command.Connection.DbConnection.ExecuteScalarAsync<T>(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Return the dataset</returns>
        public virtual async Task<DataSet> QueryMultipleAsync(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var reader = await command.Connection.DbConnection.ExecuteReaderAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var dataSet = new DataSet();
                do
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataSet.Tables.Add(dataTable);
                } while (!reader.IsClosed && reader.Read());
                return dataSet;
            }
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>>(firstDatas, secondDatas);
            }
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                var thirdDatas = gridReader.Read<TThird>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>, List<TThird>>(firstDatas, secondDatas, thirdDatas);
            }
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                var thirdDatas = gridReader.Read<TThird>().ToList();
                var fourthDatas = gridReader.Read<TFourth>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>(firstDatas, secondDatas, thirdDatas, fourthDatas);
            }
        }

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
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                var thirdDatas = gridReader.Read<TThird>().ToList();
                var fourthDatas = gridReader.Read<TFourth>().ToList();
                var fifthDatas = gridReader.Read<TFifth>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas);
            }
        }

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
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                var thirdDatas = gridReader.Read<TThird>().ToList();
                var fourthDatas = gridReader.Read<TFourth>().ToList();
                var fifthDatas = gridReader.Read<TFifth>().ToList();
                var sixthDatas = gridReader.Read<TSixth>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas, sixthDatas);
            }
        }

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
        public virtual async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = await command.Connection.DbConnection.QueryMultipleAsync(GetCommandDefinition(command, queryStatement)).ConfigureAwait(false))
            {
                var firstDatas = gridReader.Read<TFirst>().ToList();
                var secondDatas = gridReader.Read<TSecond>().ToList();
                var thirdDatas = gridReader.Read<TThird>().ToList();
                var fourthDatas = gridReader.Read<TFourth>().ToList();
                var fifthDatas = gridReader.Read<TFifth>().ToList();
                var sixthDatas = gridReader.Read<TSixth>().ToList();
                var seventhDatas = gridReader.Read<TSeventh>().ToList();
                return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas, sixthDatas, seventhDatas);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert data and return auto identities
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Added data identities,Key: command id, Value: identity value</returns>
        public virtual async Task<Dictionary<string, TIdentity>> InsertAndReturnIdentityAsync<TIdentity>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseExecutionStatements(command);
            var identityDict = new Dictionary<string, TIdentity>();
            var dbConnection = command.Connection.DbConnection;
            foreach (var statement in statements)
            {
                using (var reader = await dbConnection.ExecuteReaderAsync(GetCommandDefinition(command, statement)).ConfigureAwait(false))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    if (dataTable.Rows.Count > 0)
                    {
                        var firstRow = dataTable.Rows[0];
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            identityDict[col.ColumnName] = firstRow[col].ConvertTo<TIdentity>();
                        }
                    }
                }
            }
            return identityDict;
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="command">Database bulk insert command</param>
        public abstract Task BulkInsertAsync(BulkInsertDatabaseCommand command);

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="command">command</param>
        /// <returns></returns>
        public virtual async Task MigrateAsync(MigrationDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseMigrationStatements(command);
            foreach (var statement in statements)
            {
                await ExecuteDatabaseStatementAsync(command, statement).ConfigureAwait(false);
            }
        }

        #endregion

        #region Get table

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public virtual async Task<List<SixnetDataTable>> GetTablesAsync(SixnetDatabaseCommand command)
        {
            return (await command.Connection.DbConnection.QueryAsync<SixnetDataTable>(queryDatabaseTablesScript, transaction: command.Connection?.Transaction?.DbTransaction).ConfigureAwait(false)).ToList();
        }

        #endregion
    }
}
