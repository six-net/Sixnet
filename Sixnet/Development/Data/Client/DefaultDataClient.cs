using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> Query<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return Query<T>(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public List<T> Query<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
            var datas = DataCommandExecutor.Query<T>(connections, command, options);

            // query callback
            HandleQueryCallback(command, datas);

            return datas;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<T> Query<T>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return Query<T>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public T QueryFirst<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return QueryFirst<T>(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public T QueryFirst<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
            var data = DataCommandExecutor.QueryFirst<T>(connections, command, options);

            // query callback
            HandleQueryCallback(command, new T[1] { data });

            return data;
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public T QueryFirst<T>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryFirst<T>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, SixnetDataOperationOptions options = null)
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
        public PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, SixnetDataOperationOptions options = null)
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
        public PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            command.PagingFilter = pagingFilter;
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        public PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, int page, int pageSize, SixnetDataOperationOptions options = null)
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(string script, object parameters, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMapping(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public bool Exists<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return Exists(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public bool Exists(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
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
        public int Count<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return Count(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Count(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
            return DataCommandExecutor.Count(connections, command, options);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Max<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public TValue Max<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            //var firstField = queryable.SelectedFields?.FirstOrDefault();
            //SixnetException.ThrowIf(!FieldFormatterNames.MAX.Equals(firstField?.FormatSetting?.Name), "The field for which the maximum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Min<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public TValue Min<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            //var firstField = queryable.SelectedFields?.FirstOrDefault();
            //SixnetException.ThrowIf(!FieldFormatterNames.MIN.Equals(firstField?.FormatSetting?.Name), "The field for which the minimum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Sum<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public TValue Sum<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            //var firstField = queryable.SelectedFields?.FirstOrDefault();
            //SixnetException.ThrowIf(!FieldFormatterNames.SUM.Equals(firstField?.FormatSetting?.Name), "The field for which the sum value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TValue Avg<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public TValue Avg<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            //var firstField = queryable.SelectedFields?.FirstOrDefault();
            //SixnetException.ThrowIf(!FieldFormatterNames.AVG.Equals(firstField?.FormatSetting?.Name), "The field for which the avg value is to be calculated is not specified");

            return Scalar<TValue>(queryable, options);
        }

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public TValue Scalar<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
            var value = DataCommandExecutor.Scalar<TValue>(connections, command, options);

            // Got value callback
            HandleGotValueCallback(command, value);

            return value;
        }

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public TValue Scalar<TValue>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return Scalar<TValue>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public DataSet QueryMultiple(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public DataSet QueryMultiple(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple(new List<ISixnetQueryable>() { GetScriptQuery(script, parameters, scriptType) }, options);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond, TThird>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond, TThird, TFourth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = GroupDataCommandsDatabaseServer(commands, true, options);
            var groupDatas = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                groupDatas.Add(DataCommandExecutor.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public int Insert<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var insertResult = InsertCore(datas, options);
            return insertResult?.Item1 ?? 0;
        }

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Insert<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return Insert<T>(new T[1] { data }, options);
        }

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public List<TIdentity> InsertReturnIdentities<T, TIdentity>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var insertResult = InsertCore(datas, options);
            return insertResult?.Item2?.Values.Cast<TIdentity>().ToList() ?? new List<TIdentity>(0);
        }

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public TIdentity InsertReturnIdentity<T, TIdentity>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var identities = InsertReturnIdentities<T, TIdentity>(new List<T>(1) { data }, options);
            if (!identities.IsNullOrEmpty())
            {
                return identities.FirstOrDefault();
            }
            return default;
        }

        /// <summary>
        /// Insert core
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<int, Dictionary<string, dynamic>> InsertCore<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }

            // Create data command
            var commands = new List<SixnetDataCommand>();
            var dataType = typeof(T);
            foreach (var data in datas)
            {
                if (data != null)
                {
                    data.OnDataAdding();

                    SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(T).Name}: {data.GetIdentityValue()} cann't to be add");

                    var addCommand = SixnetDataCommand.Create<T>(DataOperationType.Insert);
                    var valueDict = data.GetAllValues();
                    addCommand.FieldsAssignment = valueDict?.GetFieldsAssignment();
                    addCommand.Data = data;
                    commands.Add(addCommand);
                }
            }
            var incrementField = SixnetEntityManager.GetField(dataType, FieldRole.Increment);
            return ExecuteCore(commands, incrementField != null, options);
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
        public int Update<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            var commands = new List<SixnetDataCommand>();
            foreach (var newData in datas)
            {
                if (newData != null)
                {
                    var entityIdentity = newData.GetIdentityValue();
                    newData.OnDataUpdating();

                    SixnetException.ThrowIf(!newData.AllowToSave(), $"{typeof(T).Name}: {entityIdentity} cann't to be update");

                    var updateQueryable = ConditionExtensions.IncludeEntity(null, newData);
                    var fieldsAssignment = newData.GetModificationAssignment(newData);
                    var command = GetUpdateCommand(fieldsAssignment, updateQueryable, options);
                    command.Data = newData;
                    commands.Add(command);
                }
            }
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            return Update(commands);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Update<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return Update(new T[1] { data }, options);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Update<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public int Update<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            if (fieldsAssignment == null)
            {
                SixnetLogger.LogWarning<DefaultDataClient>(SixnetLogEvents.Database.NotModificationValue, "Not update any value");
                return 0;
            }

            var command = GetUpdateCommand(fieldsAssignment, queryable, options);
            return Update(new List<SixnetDataCommand>(1) { command });
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="updateCommands">Update commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Update(List<SixnetDataCommand> updateCommands)
        {
            return Execute(updateCommands);
        }

        /// <summary>
        /// Get modification command
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <returns>Return modification command</returns>
        SixnetDataCommand GetUpdateCommand(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, SixnetDataOperationOptions options)
        {
            var cmd = SixnetDataCommand.CreateQueryCommand(queryable);
            cmd.OperationType = DataOperationType.Update;
            cmd.FieldsAssignment = fieldsAssignment;
            cmd.Queryable = queryable;
            cmd.Options = options;
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
        public int Delete<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
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
        public int Delete<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return Delete(new T[1] { data }, options);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return Delete(conditionExpression.GetQueryable<T>(), options);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public int Delete(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.Create(DataOperationType.Delete, queryable);
            command.Options = options;
            return Execute(new List<SixnetDataCommand>(1) { command });
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
        Tuple<int, Dictionary<string, dynamic>> ExecuteCore(IEnumerable<SixnetDataCommand> commands, bool identityInsert, SixnetDataOperationOptions options = null)
        {
            var affectedRows = 0;
            var serverGroups = GroupDataCommandsDatabaseServer(commands, false, options);
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
        /// <returns></returns>
        public int Execute(IEnumerable<SixnetDataCommand> commands)
        {
            return ExecuteCore(commands, false).Item1;
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="command">Data command</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Execute(SixnetDataCommand command)
        {
            return Execute(new SixnetDataCommand[1] { command });
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public int Execute(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            var cmd = SixnetDataCommand.CreateScriptCommand(script, parameters, scriptType);
            cmd.Options = options;
            return Execute(new List<SixnetDataCommand>(1) { cmd });
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
            var connections = GetConnections(GetDataCommandDatabaseServers(command, false, options?.DataOperationOptions));
            DataCommandExecutor.BulkInsert(connections, dataTable, options);
        }

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public void Migrate(MigrationInfo migrationInfo, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            DataCommandExecutor.Migrate(connections, migrationInfo, options);
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public void CreateTable<TEntity>(Action<SixnetCreateTableOptions> configure = null) where TEntity : ISixnetEntity
        {
            CreateTable(typeof(TEntity), configure);
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="configure">Configure options</param>
        public void CreateTable(Type entityType, Action<SixnetCreateTableOptions> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            DataCommandExecutor.CreateTable(connections, entityType, configure);
        }

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        public List<SixnetDataTable> GetTables(SixnetDataOperationOptions options = null)
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

        #region Database

        /// <summary>
        /// Get connections
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DatabaseConnection> GetConnections()
        {
            return databaseConnections?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, DatabaseConnection>(0);
        }

        /// <summary>
        /// Get database servers
        /// </summary>
        /// <returns></returns>
        public List<DatabaseServer> GetDatabaseServers()
        {
            return internalDatabaseServers?.Select(c => c).ToList() ?? new List<DatabaseServer>(0);
        }

        #endregion

        #region Util

        /// <summary>
        /// Get database server
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="useForQuery">User for query</param>
        /// <param name="dataOperationOptions">Data operation options</param>
        /// <returns></returns>
        List<DatabaseServer> GetDataCommandDatabaseServers(SixnetDataCommand command, bool useForQuery, SixnetDataOperationOptions dataOperationOptions)
        {
            // options
            if (dataOperationOptions != null)
            {
                command.Options = dataOperationOptions;
            }

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
        Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>> GroupDataCommandsDatabaseServer(IEnumerable<SixnetDataCommand> commands, bool useForQuery, SixnetDataOperationOptions dataOperationOptions)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), nameof(commands));

            var serverGroups = new Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>>();
            foreach (var command in commands)
            {
                var databaseServers = GetDataCommandDatabaseServers(command, useForQuery, dataOperationOptions);
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
        void HandleDataCommandBeforeExecution(SixnetDataCommand dataCommand)
        {
            if (dataCommand == null)
            {
                return;
            }

            // publish starting data event
            SixnetDataEventBus.PublishStartingDataEventAsync(this, dataCommand, false, default(CancellationToken));

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
            SixnetDataEventBus.PublishQueriedEventAsync<T>(this, command, datas, default(CancellationToken));

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
            SixnetDataEventBus.PublishCheckedEventAsync(this, command, hasValue, cancellationToken);

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
            SixnetDataEventBus.PublishGotValueEventAsync(this, command, value, default(CancellationToken));

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
            SixnetDataEventBus.PublishExecutedDataEventAsync(this, command, default);

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
                    if (newValue is not ConstantField constantField || constantField.HasFormatter)
                    {
                        continue;
                    }
                    newValue = constantField.Value;
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
                    while (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        var firstTable = ds.Tables[0];
                        ds.Tables.Remove(firstTable);
                        unionDataSet.Tables.Add(firstTable);
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
        void ValidateCalculateField(ISixnetQueryable queryable, string fieldFormatterName, SixnetDataOperationOptions options)
        {
            var firstField = queryable.SelectedFields?.FirstOrDefault();
            SixnetException.ThrowIf(!string.Equals(fieldFormatterName, firstField?.FormatSetting?.Name), $"The field for which the {fieldFormatterName} value is to be calculated is not specified");
        }

        ISixnetQueryable GetScriptQuery(string script, object parameters, DataScriptType scriptType = DataScriptType.Text)
        {
            var query = SixnetQuerier.Create();
            query.SetScript(script, scriptType, parameters);
            return query;
        }

        #endregion
    }
}
