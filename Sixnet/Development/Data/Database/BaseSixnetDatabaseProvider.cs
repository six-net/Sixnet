using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Dapper;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines base database provider
    /// </summary>
    public abstract partial class BaseSixnetDatabaseProvider : ISixnetDatabaseProvider
    {
        #region Fields

        protected string queryDatabaseTablesScript = "";

        #endregion

        #region Connection

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns>Database connection</returns>
        public abstract IDbConnection GetDbConnection(SixnetDatabaseServer server);

        #endregion

        #region Command resolver

        /// <summary>
        /// Get data command resolver
        /// </summary>
        /// <returns></returns>
        protected abstract ISixnetDataCommandResolver GetDataCommandResolver();

        #endregion

        #region Parameter

        /// <summary>
        /// Convert data command parametes
        /// </summary>
        /// <param name="parameters">Data command parameters</param>
        /// <returns></returns>
        protected abstract DynamicParameters ConvertDataCommandParameters(DataCommandParameters parameters);

        #endregion

        #region Command definition

        /// <summary>
        ///  Get command definition
        /// </summary>
        /// <param name="command">Database command</param>
        /// <param name="statement">Database statement</param>
        /// <returns></returns>
        protected virtual CommandDefinition GetCommandDefinition(SixnetDatabaseCommand command, SixnetDatabaseStatement statement)
        {
            return new CommandDefinition(statement.Script, ConvertDataCommandParameters(statement.Parameters)
                                , transaction: command.Connection?.Transaction?.DbTransaction, commandType: statement.ScriptType
                                , cancellationToken: command?.CancellationToken ?? default);
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execute multiple command
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Affected data numbers</returns>
        public virtual int Execute(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseExecutionStatements(command);
            var totalAffectedNumber = 0;
            foreach (var statement in statements)
            {
                totalAffectedNumber += ExecuteDatabaseStatement(command, statement);
            }
            return totalAffectedNumber;
        }

        /// <summary>
        /// Execute database statement
        /// </summary>
        /// <param name="command">Database command</param>
        /// <param name="statement">Database execution statement</param>
        /// <returns></returns>
        protected virtual int ExecuteDatabaseStatement(SixnetDatabaseCommand command, ExecutionDatabaseStatement statement)
        {
            return command.Connection.DbConnection.Execute(GetCommandDefinition(command, statement));
        }

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual List<T> Query<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query<T>(GetCommandDefinition(command, queryStatement))?.ToList() ?? new List<T>(0);
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual T QueryFirst<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.QueryFirstOrDefault<T>(GetCommandDefinition(command, queryStatement));
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="command">Database query mapping command</param>
        /// <returns>Return the datas</returns>
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(QueryMappingDatabaseCommand<TFirst, TSecond, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
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
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
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
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
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
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
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
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
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
        public virtual List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.Query(queryStatement.Script, command.DataMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: command.SpiltOnFieldName
                , commandType: queryStatement.ScriptType
                )?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the datas</returns>
        public virtual PagingInfo<T> QueryPaging<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryPagingStatement(command);
            var pagingDatas = command.Connection.DbConnection.Query<PagingTotalCountModel, T, PagingTotalCountMappingModel<T>>(queryStatement.Script, PagingTotalCountMappingModel<T>.PagingTotalCountMappingFunc
                , param: ConvertDataCommandParameters(queryStatement.Parameters)
                , transaction: command.Connection?.Transaction?.DbTransaction
                , splitOn: SixnetDataManager.PagingTotalCountSplitFieldName
                , commandType: queryStatement.ScriptType)
                ?.ToList() ?? new List<PagingTotalCountMappingModel<T>>(0);
            if (pagingDatas.IsNullOrEmpty())
            {
                return SixnetPager.Empty<T>();
            }
            var firstData = pagingDatas.First();
            var pagingFilter = command.DataCommand.PagingFilter;
            return SixnetPager.Create(pagingFilter.Page, pagingFilter.PageSize, firstData.PagingTotalDataCount, pagingDatas.Select(c => c.RealReturnData));
        }

        /// <summary>
        /// Query whether has any data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns>Return whether the data exists or not</returns>
        public virtual bool Exists(SingleDatabaseCommand command)
        {
            command?.DataCommand?.Queryable?.Output(QueryableOutputType.Predicate);
            return Scalar<int>(command) > 0;
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public int Count(SingleDatabaseCommand command)
        {
            command?.DataCommand?.Queryable?.Output(QueryableOutputType.Count);
            return Scalar<int>(command);
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Database single command</param>
        /// <returns>Return the data</returns>
        public virtual T Scalar<T>(SingleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            return command.Connection.DbConnection.ExecuteScalar<T>(GetCommandDefinition(command, queryStatement));
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns>Return the dataset</returns>
        public virtual DataSet QueryMultiple(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var reader = command.Connection.DbConnection.ExecuteReader(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var queryStatement = dataCommandResolver.GenerateDatabaseQueryStatement(command);
            using (var gridReader = command.Connection.DbConnection.QueryMultiple(GetCommandDefinition(command, queryStatement)))
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
        public virtual Dictionary<string, TIdentity> InsertAndReturnIdentity<TIdentity>(MultipleDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseExecutionStatements(command);
            var identityDict = new Dictionary<string, TIdentity>();
            var dbConnection = command.Connection.DbConnection;
            foreach (var statement in statements)
            {
                using (var reader = dbConnection.ExecuteReader(GetCommandDefinition(command, statement)))
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
        public abstract void BulkInsert(BulkInsertDatabaseCommand command);

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="command">command</param>
        /// <returns></returns>
        public virtual void Migrate(MigrationDatabaseCommand command)
        {
            var dataCommandResolver = GetDataCommandResolver();
            var statements = dataCommandResolver.GenerateDatabaseMigrationStatements(command);
            foreach (var statement in statements)
            {
                ExecuteDatabaseStatement(command, statement);
            }
        }

        #endregion

        #region Get table

        /// <summary>
        /// Get table
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public virtual List<SixnetDataTable> GetTables(SixnetDatabaseCommand command)
        {
            return command.Connection.DbConnection.Query<SixnetDataTable>(queryDatabaseTablesScript, transaction: command.Connection?.Transaction?.DbTransaction).ToList();
        }

        #endregion
    }
}
