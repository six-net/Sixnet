using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Sixnet.Development.Command;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Event;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Logging;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Defines default data client
    /// </summary>
    internal partial class DefaultDataClient : ISixnetDataClient
    {
        #region Fields

        /// <summary>
        /// Data base connections
        /// Key: database server name
        /// Value: database connection
        /// </summary>
        readonly Dictionary<string, DatabaseConnection> databaseConnections = new();

        /// <summary>
        /// Internal database servers
        /// </summary>
        readonly List<DatabaseServer> internalDatabaseServers = null;

        /// <summary>
        /// Whether auto open connection
        /// </summary>
        readonly bool autoOpen = false;

        /// <summary>
        /// Whether auto use transaction
        /// </summary>
        readonly bool useTransaction = false;

        /// <summary>
        /// Default isolation level
        /// </summary>
        readonly DataIsolationLevel? defaultIsolationLevel;

        /// <summary>
        /// executed commands
        /// </summary>
        readonly ConcurrentQueue<SixnetDataCommand> executedCommands = new();

        /// <summary>
        /// Entity warehouse
        /// Out key: entity type guid
        /// Inner key: entity identity value
        /// </summary>
        readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>> entityWarehouse = new();

        /// <summary>
        /// Whether init by connection
        /// </summary>
        readonly bool initByConnection = false;

        #endregion

        #region Constructor

        internal DefaultDataClient(bool autoOpenConnection, bool useTransaction
            , IEnumerable<DatabaseServer> servers = null, DataIsolationLevel? isolationLevel = null)
        {
            autoOpen = autoOpenConnection;
            this.useTransaction = useTransaction;
            internalDatabaseServers = servers?.ToList();
            defaultIsolationLevel = isolationLevel;
        }

        internal DefaultDataClient(bool autoOpenConnection, DatabaseConnection connection)
        {
            initByConnection = true;
            autoOpen = autoOpenConnection;
            useTransaction = connection.UseTransaction;
            internalDatabaseServers = new List<DatabaseServer>(1) { connection.DatabaseServer };
            databaseConnections[connection.DatabaseServer.GetServerIdentityValue()] = connection;
            if (autoOpen)
            {
                connection.Open();
            }
        }

        #endregion

        #region Query

        /// <summary>
        /// Get current datas and queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentDatas"></param>
        /// <returns></returns>
        Tuple<List<T>, ISixnetQueryable> GetCurrentDatasAndQueryable<T>(IEnumerable<T> currentDatas) where T : class, ISixnetEntity<T>
        {
            var storedDatas = new List<T>();
            if (currentDatas.IsNullOrEmpty())
            {
                return new Tuple<List<T>, ISixnetQueryable>(storedDatas, null);
            }
            var notStoredDatas = new List<T>();
            foreach (var data in currentDatas)
            {
                var storedData = GetStoredEntity<T>(data.GetIdentityValue());
                if (storedData != null)
                {
                    storedDatas.Add(storedData);
                }
                else
                {
                    notStoredDatas.Add(data);
                }
            }
            var dataQueryable = notStoredDatas.IsNullOrEmpty() ? null : ConditionExtensions.IncludeEntities(null, notStoredDatas);
            return new Tuple<List<T>, ISixnetQueryable>(storedDatas, dataQueryable);
        }

        /// <summary>
        /// Query by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> QueryByCurrent<T>(IEnumerable<T> currentDatas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var storedDatasAndQueryable = GetCurrentDatasAndQueryable(currentDatas);
            if (storedDatasAndQueryable.Item2 == null)
            {
                return storedDatasAndQueryable.Item1;
            }
            storedDatasAndQueryable.Item1.AddRange(Query<T>(storedDatasAndQueryable.Item2, options) ?? new List<T>(0));
            return storedDatasAndQueryable.Item1;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> Query<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Query<T>(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public List<T> Query<T>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.Query<T>(connections, command, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public T QueryFirst<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return QueryFirst<T>(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public T QueryFirst<T>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var data = DataCommandExecutor.QueryFirst<T>(connections, command, options);

            // query callback
            HandleQueryCallback(command, new T[1] { data });

            return data;
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, DataOperationOptions options = null)
        {
            return QueryPaging<T>(conditionExpression.GetQueryable<T>(), pagingFilter, options);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, DataOperationOptions options = null)
        {
            return QueryPaging<T>(conditionExpression.GetQueryable<T>(), page, pageSize, options);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            command.PagingFilter = pagingFilter;
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var dataPaging = DataCommandExecutor.QueryPaging<T>(connections, command, options);

            // query callback
            HandleQueryCallback(command, dataPaging.Items);

            return dataPaging;
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, int page, int pageSize, DataOperationOptions options = null)
        {
            return QueryPaging<T>(queryable, PagingFilter.Create(page, pageSize), options);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
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
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
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
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
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
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var datas = DataCommandExecutor.QueryMapping(connections, command, dataMappingFunc, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public bool Exists<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Exists(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public bool Exists(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var hasData = DataCommandExecutor.Exists(connections, command, options);

            // check callback
            HandleCheckCallback(command, hasData, default);

            return hasData;
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Count(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Count(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            return DataCommandExecutor.Count(connections, command, options);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Max<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var maxQueryable = conditionExpression.GetQueryable<T>();
            maxQueryable.Select(field.GetDataField(FieldFormatterNames.MAX));
            return Max<TValue>(maxQueryable, options);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Max<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!FieldFormatterNames.MAX.Equals(firstField?.FormatSetting?.Name), "The field for which the maximum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Min<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var minQueryable = SixnetExpressionHelper.GetQueryable<T>(conditionExpression);
            minQueryable.Select(field.GetDataField(FieldFormatterNames.MIN));
            return Min<TValue>(minQueryable, options);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Min<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!FieldFormatterNames.MIN.Equals(firstField?.FormatSetting?.Name), "The field for which the minimum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Sum<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var sumQueryable = SixnetExpressionHelper.GetQueryable<T>(conditionExpression);
            sumQueryable.Select(field.GetDataField(FieldFormatterNames.SUM));
            return Sum<TValue>(sumQueryable, options);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Sum<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!FieldFormatterNames.SUM.Equals(firstField?.FormatSetting?.Name), "The field for which the sum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Avg<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var avgQueryable = SixnetExpressionHelper.GetQueryable<T>(conditionExpression);
            avgQueryable.Select(field.GetDataField(FieldFormatterNames.AVG));
            return Avg<TValue>(avgQueryable, options);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Avg<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!FieldFormatterNames.AVG.Equals(firstField?.FormatSetting?.Name), "The field for which the avg value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public TValue Scalar<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            var value = DataCommandExecutor.Scalar<TValue>(connections, command, options);

            // Got value callback
            HandleGotValueCallback(command, value);

            return value;
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public DataSet QueryMultiple(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var dataSets = new List<DataSet>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                dataSets.Add(DataCommandExecutor.QueryMultiple(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionDataSet(dataSets);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird, TFourth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
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
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
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
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            return UnionMultipleDatas(groupDatas, groupDatas?.Count ?? 0);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> Insert<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class
        {
            InsertReturnIdentities<T, dynamic>(datas, options);
            return datas.ToList();
        }

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public T Insert<T>(T data, DataOperationOptions options = null) where T : class
        {
            return Insert<T>(new T[1] { data }, options)?.FirstOrDefault();
        }

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<TIdentity> InsertReturnIdentities<T, TIdentity>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TIdentity>(0);
            }

            // Create data command
            var commands = new List<SixnetDataCommand>();
            var dataType = typeof(T);
            var isEntity = typeof(ISixnetEntity).IsAssignableFrom(dataType);
            foreach (var data in datas)
            {
                var addCommand = SixnetDataCommand.Create<T>(DataOperationType.Insert);
                var valueDict = isEntity ? ((ISixnetEntity)data).GetAllValues() : data.ToDynamicDictionary();
                addCommand.FieldsAssignment = valueDict?.GetFieldsAssignment();
                addCommand.Data = data;
                commands.Add(addCommand);
            }
            var incrementField = SixnetEntityManager.GetField(dataType, FieldRole.Increment);
            var executeResult = ExecuteCore(commands, incrementField != null, options);
            return executeResult.Item2?.Values.Cast<TIdentity>().ToList() ?? new List<TIdentity>(0);
        }

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TIdentity InsertReturnIdentity<T, TIdentity>(T data, DataOperationOptions options = null) where T : class
        {
            var identities = InsertReturnIdentities<T, TIdentity>(new List<T>(1) { data }, options);
            if (!identities.IsNullOrEmpty())
            {
                return identities.FirstOrDefault();
            }
            return default;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> Update<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<T>(0);
            }
            var commands = new List<SixnetDataCommand>();
            foreach (var newData in datas)
            {
                if (newData == null)
                {
                    continue;
                }
                var updateQueryable = ConditionExtensions.IncludeEntity(null, newData);
                var entityIdentity = newData.GetIdentityValue();

                // get client stored data
                var oldData = GetStoredEntity<T>(entityIdentity);
                var fieldsAssignment = newData.GetFieldsAssignment(oldData?.GetAllValues());

                var command = GetUpdateCommand(fieldsAssignment, updateQueryable, options);
                command.Data = newData;
                commands.Add(command);
            }
            if (!commands.IsNullOrEmpty())
            {
                Update(commands, options);
            }
            return datas.ToList();
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public T Update<T>(T data, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return Update(new T[1] { data }, options)?.FirstOrDefault();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Update<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Update(fieldsAssignmentExpression.GetFieldsAssignment(), conditionExpression, options);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Update<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Update(fieldsAssignment, conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            if (fieldsAssignment == null)
            {
                SixnetLogger.LogWarning<DefaultDataClient>(SixnetLogEvents.Database.NotModificationValue, "Not update any value");
                return 0;
            }

            var command = GetUpdateCommand(fieldsAssignment, queryable, options);
            return Update(new List<SixnetDataCommand>(1) { command }, options);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="updateCommands">Update commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Update(List<SixnetDataCommand> updateCommands, DataOperationOptions options = null)
        {
            return Execute(updateCommands, options);
        }

        /// <summary>
        /// Get modification command
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <returns>Return modification command</returns>
        SixnetDataCommand GetUpdateCommand(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, DataOperationOptions options)
        {
            var cmd = SixnetDataCommand.CreateQueryCommand(queryable);
            cmd.OperationType = DataOperationType.Update;
            cmd.FieldsAssignment = fieldsAssignment;
            cmd.Queryable = queryable;
            return cmd;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            var queryable = ConditionExtensions.IncludeEntities(null, datas);
            return Delete(queryable, options);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete<T>(T data, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return Delete(new T[1] { data }, options);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return Delete(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.Create(DataOperationType.Delete, queryable);
            return Execute(new List<SixnetDataCommand>(1) { command }, options);
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execute core
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="identityInsert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Tuple<int, Dictionary<string, dynamic>> ExecuteCore(IEnumerable<SixnetDataCommand> commands, bool identityInsert, DataOperationOptions options = null)
        {
            var affectedRows = 0;
            var serverGroups = GroupDataCommandsDatabaseServer(commands, false);
            var identities = new Dictionary<string, dynamic>();
            if (identityInsert)
            {
                foreach (var serverItem in serverGroups)
                {
                    var serverConnection = GetConnection(serverItem.Value.Item1);
                    var serverIdentities = DataCommandExecutor.InsertAndReturnAutoIdentity<dynamic>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options);
                    if (!serverIdentities.IsNullOrEmpty())
                    {
                        foreach (var identityItem in serverIdentities)
                        {
                            identities[identityItem.Key] = identityItem.Value;
                        }
                    }
                }
                affectedRows = identities.Count;
            }
            else
            {
                foreach (var serverItem in serverGroups)
                {
                    var serverConnection = GetConnection(serverItem.Value.Item1);
                    affectedRows += DataCommandExecutor.Execute(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options);
                }
            }

            // handle command callback
            HandleExecutionCallback(commands, identities);

            return new Tuple<int, Dictionary<string, dynamic>>(affectedRows, identities);
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Execute(IEnumerable<SixnetDataCommand> commands, DataOperationOptions options = null)
        {
            return ExecuteCore(commands, false, options).Item1;
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="command">Data command</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Execute(SixnetDataCommand command, DataOperationOptions options = null)
        {
            return Execute(new SixnetDataCommand[1] { command }, options);
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Execute(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, DataOperationOptions options = null)
        {
            var cmd = SixnetDataCommand.CreateScriptCommand(script, parameters, scriptType);
            return Execute(new List<SixnetDataCommand>(1) { cmd }, options);
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public void BulkInsert(DataTable dataTable, ISixnetBulkInsertionOptions options = null)
        {
            var command = SixnetDataCommand.Create(dataTable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, false));
            DataCommandExecutor.BulkInsert(connections, dataTable, options);
        }

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public void Migrate(MigrationInfo migrationInfo, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            DataCommandExecutor.Migrate(connections, migrationInfo, options);
        }

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        public List<SixnetDataTable> GetTables(DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return DataCommandExecutor.GetTables(connections?.FirstOrDefault(), options);
        }

        #endregion

        #region Open

        /// <summary>
        /// Open data client
        /// </summary>
        public void Open()
        {
            foreach (var conn in databaseConnections.Values)
            {
                conn?.Open();
            }
        }

        #endregion

        #region Close

        /// <summary>
        /// Close data client
        /// </summary>
        public void Close()
        {
            if (!initByConnection)
            {
                foreach (var conn in databaseConnections.Values)
                {
                    conn?.Close();
                }
            }
            Reset();
        }

        #endregion

        #region Transaction

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            CommitAsync().Wait();
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            if (!initByConnection)
            {
                foreach (var conn in databaseConnections.Values)
                {
                    conn?.Rollback();
                }
            }
            Reset();
        }

        #endregion

        #region Dispose

        /// <summary>
        ///  Dispose client
        /// </summary>
        public void Dispose()
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<DefaultDataClient>(ex, ex.Message);
            }
        }

        #endregion

        #region Entity warehouse

        /// <summary>
        /// Store entity
        /// </summary>
        /// <param name="entity">Entity</param>
        internal void StoreEntity<TEntity>(TEntity entity) where TEntity : class, ISixnetEntity
        {
            if (entity == null)
            {
                return;
            }
            var entityKey = entity.GetIdentityValue();
            if (!string.IsNullOrWhiteSpace(entityKey))
            {
                var entityTypeId = entity.GetType().GUID;
                entityWarehouse.TryGetValue(entityTypeId, out var entityDict);
                entityDict ??= new ConcurrentDictionary<string, object>();
                entityDict[entityKey] = entity;
                entityWarehouse[entityTypeId] = entityDict;
            }
        }

        /// <summary>
        /// Get entity
        /// </summary>
        /// <param name="entityIdentity">Entity identity</param>
        /// <returns></returns>
        internal TEntity GetStoredEntity<TEntity>(string entityIdentity) where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(entityIdentity))
            {
                return default;
            }
            var entityTypeId = typeof(TEntity).GUID;
            if (entityWarehouse.TryGetValue(entityTypeId, out var entityDict) && entityDict.TryGetValue(entityIdentity, out var entity))
            {
                return (TEntity)entity;
            }
            return default;
        }

        /// <summary>
        /// Delete stored entity
        /// </summary>
        /// <param name="entity">Entity</param>
        internal void DeleteStoredEntity<TEntity>(TEntity entity) where TEntity : class, ISixnetEntity
        {
            if (entity == null)
            {
                return;
            }
            var entityIdentity = entity.GetIdentityValue();
            var entityTypeId = entity.GetType().GUID;
            if (!string.IsNullOrWhiteSpace(entityIdentity) && entityWarehouse.TryGetValue(entityTypeId, out var entityDict))
            {
                entityDict.TryRemove(entityIdentity, out var _);
            }
        }

        /// <summary>
        /// Delete stored entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        internal void DeleteStoredEntity<TEntity>(ISixnetQueryable queryable) where TEntity : class
        {
            if (queryable == null || queryable.Conditions.IsNullOrEmpty() || queryable.IsComplex)
            {
                ClearStoredEntity<TEntity>();
            }
            else
            {
                var entityTypeId = typeof(TEntity).GUID;
                if (entityWarehouse.TryGetValue(entityTypeId, out var entityDict))
                {
                    var validationFunc = queryable.GetValidationFunction<TEntity>();
                    var entityKeies = entityDict.Keys;
                    foreach (var entityKey in entityKeies)
                    {
                        if (entityDict.TryGetValue(entityKey, out var data) && validationFunc(data as TEntity))
                        {
                            entityDict.TryRemove(entityKey, out var _);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        internal void ClearStoredEntity<TEntity>() where TEntity : class
        {
            var entityTypeId = typeof(TEntity).GUID;
            entityWarehouse.TryRemove(entityTypeId, out var _);
        }

        #endregion

        #region Util

        /// <summary>
        /// Get database server
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        List<DatabaseServer> GetDataCommandDatabaseServers(SixnetDataCommand command, bool useForQuery)
        {
            var servers = internalDatabaseServers.IsNullOrEmpty()
                ? SixnetDataManager.GetCommandDatabaseServers(command)
                : internalDatabaseServers;

            // handle data command before execution
            HandleDataCommandBeforeExecution(command);

            return servers;
        }

        /// <summary>
        /// Group data commands server
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Key: database server name,Value: commands</returns>
        Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>> GroupDataCommandsDatabaseServer(IEnumerable<SixnetDataCommand> commands, bool useForQuery)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), nameof(commands));

            var serverGroups = new Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>>();
            foreach (var command in commands)
            {
                var databaseServers = GetDataCommandDatabaseServers(command, useForQuery);
                if (!databaseServers.IsNullOrEmpty())
                {
                    foreach (var server in databaseServers)
                    {
                        serverGroups.TryGetValue(server.Name, out var serverCommands);
                        serverCommands ??= new Tuple<DatabaseServer, List<SixnetDataCommand>>(server, new List<SixnetDataCommand>());
                        serverCommands.Item2.Add(command);
                        serverGroups[server.Name] = serverCommands;
                    }
                }
            }
            return serverGroups;
        }

        /// <summary>
        /// Handle data command before execution
        /// </summary>
        /// <param name="dataCommand">Data command</param>
        /// <param name="useForQuery">Use for query</param>
        void HandleDataCommandBeforeExecution(SixnetDataCommand dataCommand)
        {
            if (dataCommand == null)
            {
                return;
            }

            // publish starting data event
            SixnetDataEventBus.PublishStartingDataEvent(this, dataCommand, false, default(CancellationToken));

            if (dataCommand.ExecutionMode == CommandExecutionMode.Transform)
            {
                #region Delete

                var queryable = dataCommand.Queryable;
                var options = dataCommand.Options;
                var entityType = dataCommand.GetEntityType();
                if (dataCommand.OperationType == DataOperationType.Delete && entityType != null && (queryable == null || queryable.ExecutionMode == QueryableExecutionMode.Regular))
                {
                    #region Logic delete

                    if (SixnetDataManager.AllowLogicalDelete(options))
                    {
                        var archiveFieldName = SixnetEntityManager.GetFieldName(entityType, FieldRole.Archive);
                        if (!string.IsNullOrWhiteSpace(archiveFieldName))
                        {
                            var fieldsAssignment = FieldsAssignment.Create();
                            fieldsAssignment.SetNewValue(archiveFieldName, true);
                            dataCommand.OperationType = DataOperationType.Update;
                            dataCommand.FieldsAssignment = fieldsAssignment;
                        }
                    }

                    #endregion
                }

                #endregion
            }

            // trigger data command starting event
            SixnetDataManager.TriggerDataCommandStartingEvent(dataCommand);
        }

        /// <summary>
        /// Handle query callback
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <param name="datas">Datas</param>
        void HandleQueryCallback<T>(SixnetDataCommand command, IEnumerable<T> datas)
        {
            // data queried event
            SixnetDataEventBus.PublishQueriedEvent<T>(this, command, datas, default(CancellationToken));

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle check callback
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="hasValue">Has value</param>
        void HandleCheckCallback(SixnetDataCommand command, bool hasValue, CancellationToken cancellationToken)
        {
            // checked event
            SixnetDataEventBus.PublishCheckedEvent(this, command, hasValue, cancellationToken);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle got value callback
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="value">Value</param>
        void HandleGotValueCallback(SixnetDataCommand command, dynamic value)
        {
            // Got event
            SixnetDataEventBus.PublishGotValueEvent(this, command, value, default(CancellationToken));

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle execution callback
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        void HandleExecutionCallback(IEnumerable<SixnetDataCommand> commands, Dictionary<string, dynamic> identities)
        {
            if (!commands.IsNullOrEmpty())
            {
                foreach (var command in commands)
                {
                    HandleExecutionCallback(command, identities);
                }
            }
        }

        /// <summary>
        /// Handle execution callback
        /// </summary>
        /// <param name="command">Command</param>
        void HandleExecutionCallback(SixnetDataCommand command, Dictionary<string, dynamic> identities)
        {
            // update object
            switch (command.OperationType)
            {
                case DataOperationType.Insert:
                case DataOperationType.Update:
                    UpdateObject(command, identities);
                    break;
            }

            if (useTransaction)
            {
                executedCommands.Enqueue(command);
            }
            else
            {
                ExecuteDataCommandCallback(command);
            }
        }

        /// <summary>
        /// Hanle queued execution callback
        /// </summary>
        void HandleQueuedExecutionCallback()
        {
            while (executedCommands.TryDequeue(out var command))
            {
                ExecuteDataCommandCallback(command);
            }
        }

        /// <summary>
        /// Execute data command callback
        /// </summary>
        /// <param name="command">Command</param>
        void ExecuteDataCommandCallback(SixnetDataCommand command)
        {
            // callback data event
            SixnetDataEventBus.PublishExecutedDataEvent(this, command, default);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns></returns>
        DatabaseConnection GetConnection(DatabaseServer server)
        {
            var serverIdentity = server.GetServerIdentityValue();
            if (!databaseConnections.TryGetValue(serverIdentity ?? string.Empty, out var conn))
            {
                lock (databaseConnections)
                {
                    if (!databaseConnections.TryGetValue(serverIdentity, out conn))
                    {
                        conn = DatabaseConnection.Create(server, useTransaction, defaultIsolationLevel);
                        if (autoOpen)
                        {
                            conn.Open();
                        }
                        databaseConnections[serverIdentity] = conn;
                    }
                }
            }
            return conn;
        }

        /// <summary>
        /// Get database connections
        /// </summary>
        /// <param name="servers">Database servers</param>
        /// <returns></returns>
        List<DatabaseConnection> GetConnections(IEnumerable<DatabaseServer> servers)
        {
            if (servers.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(servers));
            }
            return servers.Select(c => GetConnection(c)).ToList();
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="dataCommand">Data command</param>
        /// <param name="identities">Identities</param>
        void UpdateObject(SixnetDataCommand dataCommand, Dictionary<string, dynamic> identities = null)
        {
            if (dataCommand?.Data is not ISixnetEntity)
            {
                return;
            }
            var entity = dataCommand.Data as ISixnetEntity;
            foreach (var newValueItem in dataCommand.FieldsAssignment.NewValues)
            {
                var newValue = newValueItem.Value;
                if (newValue is ISixnetField)
                {
                    if (newValue is ConstantField constantField && !constantField.HasFormatter)
                    {
                        newValue = constantField.Value;
                    }
                    else
                    {
                        continue;
                    }
                }
                var propertyName = newValueItem.Key;
                entity.SetValue(propertyName, newValue);
            }

            if (!identities.IsNullOrEmpty())
            {
                var dataType = entity.GetType();
                var autoIdentityFieldName = SixnetEntityManager.GetFieldName(dataType, FieldRole.Increment);
                if (!string.IsNullOrWhiteSpace(autoIdentityFieldName) && identities.TryGetValue(dataCommand.Id, out var autoIdentityValue))
                {
                    var autoIdentityField = SixnetEntityManager.GetField(dataType, autoIdentityFieldName);
                    entity.SetValue(autoIdentityFieldName, ObjectExtensions.ConvertTo(autoIdentityValue, autoIdentityField.DataType));
                }
            }
        }

        /// <summary>
        /// Reset client
        /// </summary>
        void Reset()
        {
            executedCommands?.Clear();
        }

        /// <summary>
        /// Union data set
        /// </summary>
        /// <param name="dataSets"></param>
        /// <returns></returns>
        DataSet UnionDataSet(IEnumerable<DataSet> dataSets)
        {
            var unionDataSet = new DataSet();
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var ds in dataSets)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            unionDataSet.Tables.Add(dt);
                        }
                    }
                }
            }
            return unionDataSet;
        }

        Tuple<List<TFirst>, List<TSecond>> UnionMultipleDatas<TFirst, TSecond>(IEnumerable<Tuple<List<TFirst>, List<TSecond>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>>(finallyFirstDatas, finallySecondDatas);
        }

        Tuple<List<TFirst>, List<TSecond>, List<TThird>> UnionMultipleDatas<TFirst, TSecond, TThird>(IEnumerable<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            var finallyThirdDatas = new List<TThird>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                    if (!taskDataItem.Item3.IsNullOrEmpty())
                    {
                        finallyThirdDatas.AddRange(taskDataItem.Item3);
                    }
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>>(finallyFirstDatas, finallySecondDatas, finallyThirdDatas);
        }

        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> UnionMultipleDatas<TFirst, TSecond, TThird, TFourth>(IEnumerable<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            var finallyThirdDatas = new List<TThird>();
            var finallyFourthDatas = new List<TFourth>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                    if (!taskDataItem.Item3.IsNullOrEmpty())
                    {
                        finallyThirdDatas.AddRange(taskDataItem.Item3);
                    }
                    if (!taskDataItem.Item4.IsNullOrEmpty())
                    {
                        finallyFourthDatas.AddRange(taskDataItem.Item4);
                    }
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>(finallyFirstDatas, finallySecondDatas, finallyThirdDatas, finallyFourthDatas);
        }

        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> UnionMultipleDatas<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            var finallyThirdDatas = new List<TThird>();
            var finallyFourthDatas = new List<TFourth>();
            var finallyFifthDatas = new List<TFifth>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                    if (!taskDataItem.Item3.IsNullOrEmpty())
                    {
                        finallyThirdDatas.AddRange(taskDataItem.Item3);
                    }
                    if (!taskDataItem.Item4.IsNullOrEmpty())
                    {
                        finallyFourthDatas.AddRange(taskDataItem.Item4);
                    }
                    if (!taskDataItem.Item5.IsNullOrEmpty())
                    {
                        finallyFifthDatas.AddRange(taskDataItem.Item5);
                    }
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>(finallyFirstDatas, finallySecondDatas, finallyThirdDatas, finallyFourthDatas, finallyFifthDatas);
        }

        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> UnionMultipleDatas<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            var finallyThirdDatas = new List<TThird>();
            var finallyFourthDatas = new List<TFourth>();
            var finallyFifthDatas = new List<TFifth>();
            var finallySixthDatas = new List<TSixth>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                    if (!taskDataItem.Item3.IsNullOrEmpty())
                    {
                        finallyThirdDatas.AddRange(taskDataItem.Item3);
                    }
                    if (!taskDataItem.Item4.IsNullOrEmpty())
                    {
                        finallyFourthDatas.AddRange(taskDataItem.Item4);
                    }
                    if (!taskDataItem.Item5.IsNullOrEmpty())
                    {
                        finallyFifthDatas.AddRange(taskDataItem.Item5);
                    }
                    if (!taskDataItem.Item6.IsNullOrEmpty())
                    {
                        finallySixthDatas.AddRange(taskDataItem.Item6);
                    }
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>(finallyFirstDatas, finallySecondDatas, finallyThirdDatas, finallyFourthDatas, finallyFifthDatas, finallySixthDatas);
        }

        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> UnionMultipleDatas<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> datas, int groupCount)
        {
            if (groupCount < 2 || datas.IsNullOrEmpty())
            {
                return datas?.FirstOrDefault();
            }
            var finallyFirstDatas = new List<TFirst>();
            var finallySecondDatas = new List<TSecond>();
            var finallyThirdDatas = new List<TThird>();
            var finallyFourthDatas = new List<TFourth>();
            var finallyFifthDatas = new List<TFifth>();
            var finallySixthDatas = new List<TSixth>();
            var finallySeventhDatas = new List<TSeventh>();
            foreach (var taskDataItem in datas)
            {
                if (taskDataItem != null)
                {
                    if (!taskDataItem.Item1.IsNullOrEmpty())
                    {
                        finallyFirstDatas.AddRange(taskDataItem.Item1);
                    }
                    if (!taskDataItem.Item2.IsNullOrEmpty())
                    {
                        finallySecondDatas.AddRange(taskDataItem.Item2);
                    }
                    if (!taskDataItem.Item3.IsNullOrEmpty())
                    {
                        finallyThirdDatas.AddRange(taskDataItem.Item3);
                    }
                    if (!taskDataItem.Item4.IsNullOrEmpty())
                    {
                        finallyFourthDatas.AddRange(taskDataItem.Item4);
                    }
                    if (!taskDataItem.Item5.IsNullOrEmpty())
                    {
                        finallyFifthDatas.AddRange(taskDataItem.Item5);
                    }
                    if (!taskDataItem.Item6.IsNullOrEmpty())
                    {
                        finallySixthDatas.AddRange(taskDataItem.Item6);
                    }
                    if (!taskDataItem.Item7.IsNullOrEmpty())
                    {
                        finallySeventhDatas.AddRange(taskDataItem.Item7);
                    }
                }
            }

            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>(
                finallyFirstDatas, finallySecondDatas, finallyThirdDatas, finallyFourthDatas, finallyFifthDatas, finallySixthDatas, finallySeventhDatas);
        }

        /// <summary>
        /// Validate calculate field
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="fieldFormatterName">Field formatter name</param>
        /// <param name="options">Options</param>
        void ValidateCalculateField(ISixnetQueryable queryable, string fieldFormatterName, DataOperationOptions options)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!string.Equals(fieldFormatterName, firstField?.FormatSetting?.Name), $"The field for which the {fieldFormatterName} value is to be calculated is not specified");
        }

        #endregion
    }
}
