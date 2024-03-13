using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Command;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Event;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Defines default data client
    /// </summary>
    internal partial class DefaultDataClient
    {
        #region Query

        /// <summary>
        /// Query by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<List<T>> QueryByCurrentAsync<T>(IEnumerable<T> currentDatas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var storedDatasAndQueryable = GetCurrentDatasAndQueryable(currentDatas);
            if (storedDatasAndQueryable.Item2 != null)
            {
                storedDatasAndQueryable.Item1.AddRange((await QueryAsync<T>(storedDatasAndQueryable.Item2, options).ConfigureAwait(false)) ?? new List<T>(0));
            }
            return storedDatasAndQueryable.Item1;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await QueryAsync<T>(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public async Task<List<T>> QueryAsync<T>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryAsync<T>(connections, command, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return datas;
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await QueryFirstAsync<T>(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public async Task<T> QueryFirstAsync<T>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var data = await DataCommandExecutor.QueryFirstAsync<T>(connections, command, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, new List<T>(1) { data }, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return data;
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, DataOperationOptions options = null)
        {
            return await QueryPagingAsync(conditionExpression, PagingFilter.Create(page, pageSize), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, DataOperationOptions options = null)
        {
            return await QueryPagingAsync<T>(conditionExpression.GetQueryable<T>(), pagingFilter, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        public async Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, int page, int pageSize, DataOperationOptions options = null)
        {
            return await QueryPagingAsync<T>(queryable, PagingFilter.Create(page, pageSize), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        public async Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            command.PagingFilter = pagingFilter;
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryPagingAsync<T>(connections, command, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas.Items, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return datas;
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var datas = await DataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return datas;
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await ExistsAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query whether has any data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public async Task<bool> ExistsAsync(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var hasData = await DataCommandExecutor.ExistsAsync(connections, command, options).ConfigureAwait(false);

            // check callback
            await HandleCheckCallbackAsync(command, hasData, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return hasData;
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await CountAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> CountAsync(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true));
            return await DataCommandExecutor.CountAsync(connections, command, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MaxAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var maxQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(FieldFormatterNames.MAX));
            return await MaxAsync<TValue>(maxQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, FieldFormatterNames.MAX, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MinAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var minQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(FieldFormatterNames.MIN));
            return await MinAsync<TValue>(minQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, FieldFormatterNames.MIN, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> SumAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var sumQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(FieldFormatterNames.SUM));
            return await SumAsync<TValue>(sumQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, FieldFormatterNames.SUM, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> AvgAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            var avgQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(FieldFormatterNames.AVG));
            return await AvgAsync<TValue>(avgQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, FieldFormatterNames.AVG, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public async Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true).ConfigureAwait(false));
            var value = await DataCommandExecutor.ScalarAsync<TValue>(connections, command, options).ConfigureAwait(false);

            // Got value callback
            await HandleGotValueCallbackAsync(command, value, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return value;
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public async Task<DataSet> QueryMultipleAsync(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<DataSet>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDataSets = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionDataSet(taskDataSets);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(DataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
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
        public async Task<List<T>> InsertAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class
        {
            await InsertReturnIdentitiesAsync<T, dynamic>(datas, options).ConfigureAwait(false);
            return datas.ToList();
        }

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<List<TIdentity>> InsertReturnIdentitiesAsync<T, TIdentity>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class
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
            var executeResult = await ExecuteCoreAsync(commands, incrementField != null, options).ConfigureAwait(false);
            return executeResult.Item2?.Values.Cast<TIdentity>().ToList() ?? new List<TIdentity>(0);
        }

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<T> InsertAsync<T>(T data, DataOperationOptions options = null) where T : class
        {
            var datas = await InsertAsync<T>(new List<T>(1) { data }, options).ConfigureAwait(false);
            return datas?.FirstOrDefault();
        }

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TIdentity> InsertReturnIdentityAsync<T, TIdentity>(T data, DataOperationOptions options = null) where T : class
        {
            var identities = await InsertReturnIdentitiesAsync<T, TIdentity>(new List<T>(1) { data }, options).ConfigureAwait(false);
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
        public async Task<List<T>> UpdateAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
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
                await UpdateAsync(commands, options).ConfigureAwait(false);
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
        public async Task<T> UpdateAsync<T>(T data, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return (await UpdateAsync(new List<T>() { data }, options).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await UpdateAsync(fieldsAssignmentExpression.GetFieldsAssignment(), conditionExpression, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> UpdateAsync<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await UpdateAsync(fieldsAssignment, conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            if (fieldsAssignment == null)
            {
                return 0;
            }

            var command = GetUpdateCommand(fieldsAssignment, queryable, options);
            return await UpdateAsync(new List<SixnetDataCommand>(1) { command }, options).ConfigureAwait(false);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="updateCommands">Update commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        async Task<int> UpdateAsync(List<SixnetDataCommand> updateCommands, DataOperationOptions options = null)
        {
            return await ExecuteAsync(updateCommands, options).ConfigureAwait(false);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            var queryable = ConditionExtensions.IncludeEntities(null, datas);
            var affectedRows = await DeleteAsync(queryable, options).ConfigureAwait(false);
            return affectedRows;
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(T data, DataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return await DeleteAsync(new List<T>(1) { data }, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null)
        {
            return await DeleteAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync(ISixnetQueryable queryable, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.Create(DataOperationType.Delete, queryable);
            return await ExecuteAsync(new List<SixnetDataCommand>(1) { command }, options).ConfigureAwait(false);
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
        async Task<Tuple<int, Dictionary<string, dynamic>>> ExecuteCoreAsync(IEnumerable<SixnetDataCommand> commands, bool identityInsert, DataOperationOptions options = null)
        {
            var affectedRows = 0;
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, false).ConfigureAwait(false);
            Dictionary<string, dynamic> identities = null;
            if (identityInsert)
            {
                var executionTasks = new List<Task<Dictionary<string, dynamic>>>(serverGroups.Count);
                foreach (var serverItem in serverGroups)
                {
                    var serverConnection = GetConnection(serverItem.Value.Item1);
                    executionTasks.Add(DataCommandExecutor.InsertAndReturnAutoIdentityAsync<dynamic>(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
                }
                identities = (await Task.WhenAll(executionTasks).ConfigureAwait(false))?
                    .SelectMany(c => c)
                    .GroupBy(c => c.Key, c => c.Value)
                    .ToDictionary(c => c.Key, c => c.FirstOrDefault()) ?? new Dictionary<string, dynamic>(0);

                affectedRows = identities.Count;
            }
            else
            {
                var executionTasks = new List<Task<int>>(serverGroups.Count);
                foreach (var serverItem in serverGroups)
                {
                    var serverConnection = GetConnection(serverItem.Value.Item1);
                    executionTasks.Add(DataCommandExecutor.ExecuteAsync(new List<DatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
                }
                var taskDatas = await Task.WhenAll(executionTasks).ConfigureAwait(false);
                affectedRows = taskDatas?.Sum(c => c) ?? 0;
            }

            // handle command callback
            await HandleExecutionCallbackAsync(commands, identities).ConfigureAwait(false);

            return new Tuple<int, Dictionary<string, dynamic>>(affectedRows, identities);
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(IEnumerable<SixnetDataCommand> commands, DataOperationOptions options = null)
        {
            return (await ExecuteCoreAsync(commands, false, options).ConfigureAwait(false)).Item1;
        }


        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, DataOperationOptions options = null)
        {
            var cmd = SixnetDataCommand.CreateScriptCommand(script, parameters, scriptType);
            return await ExecuteAsync(new List<SixnetDataCommand>(1) { cmd }, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task BulkInsertAsync(DataTable dataTable, ISixnetBulkInsertionOptions options = null)
        {
            var command = SixnetDataCommand.Create(dataTable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, false).ConfigureAwait(false));
            await DataCommandExecutor.BulkInsertAsync(connections, dataTable, options).ConfigureAwait(false);
        }

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public Task MigrateAsync(MigrationInfo migrationInfo, DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return DataCommandExecutor.MigrateAsync(connections, migrationInfo, options);
        }

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        public Task<List<SixnetDataTable>> GetTablesAsync(DataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return DataCommandExecutor.GetTablesAsync(connections?.FirstOrDefault(), options);
        }

        #endregion

        #region Transaction

        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (!initByConnection)
            {
                foreach (var conn in databaseConnections.Values)
                {
                    conn?.Commit();
                }
            }

            // Data command callback event
            await HandleQueuedExecutionCallbackAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Util

        /// <summary>
        /// Get database server
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        async Task<List<DatabaseServer>> GetDataCommandDatabaseServersAsync(SixnetDataCommand command, bool useForQuery)
        {
            var servers = internalDatabaseServers.IsNullOrEmpty()
                ? SixnetDataManager.GetCommandDatabaseServers(command)
                : internalDatabaseServers;

            // handle data command before execution
            await HandleDataCommandBeforeExecutionAsync(command, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return servers;
        }

        /// <summary>
        /// Group data commands server
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Key: database server name,Value: commands</returns>
        async Task<Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>>> GroupDataCommandsDatabaseServerAsync(IEnumerable<SixnetDataCommand> commands, bool useForQuery)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), nameof(commands));

            var serverGroups = new Dictionary<string, Tuple<DatabaseServer, List<SixnetDataCommand>>>();
            foreach (var command in commands)
            {
                var databaseServers = await GetDataCommandDatabaseServersAsync(command, useForQuery).ConfigureAwait(false);
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
        async Task HandleDataCommandBeforeExecutionAsync(SixnetDataCommand dataCommand, CancellationToken cancellationToken)
        {
            if (dataCommand == null)
            {
                return;
            }

            // publish starting data event
            await SixnetDataEventBus.PublishStartingDataEvent(this, dataCommand, true, cancellationToken).ConfigureAwait(false);

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
        async Task HandleQueryCallbackAsync<T>(SixnetDataCommand command, IEnumerable<T> datas, CancellationToken cancellationToken)
        {
            // data queried event
            await SixnetDataEventBus.PublishQueriedEvent(this, command, datas, cancellationToken).ConfigureAwait(false);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle check callback
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="hasValue">Has value</param>
        async Task HandleCheckCallbackAsync(SixnetDataCommand command, bool hasValue, CancellationToken cancellationToken)
        {
            // checked event
            await SixnetDataEventBus.PublishCheckedEvent(this, command, hasValue, cancellationToken).ConfigureAwait(false);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle got value callback
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="value">Value</param>
        async Task HandleGotValueCallbackAsync(SixnetDataCommand command, dynamic value, CancellationToken cancellationToken)
        {
            // Got event
            await SixnetDataEventBus.PublishGotValueEvent(this, command, value, cancellationToken).ConfigureAwait(false);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        /// <summary>
        /// Handle execution callback
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        async Task HandleExecutionCallbackAsync(IEnumerable<SixnetDataCommand> commands, Dictionary<string, dynamic> identities)
        {
            if (!commands.IsNullOrEmpty())
            {
                var executionTasks = new List<Task>();
                foreach (var command in commands)
                {
                    executionTasks.Add(HandleExecutionCallbackAsync(command, identities));
                }
                await Task.WhenAll(executionTasks).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handle execution callback
        /// </summary>
        /// <param name="command">Command</param>
        async Task HandleExecutionCallbackAsync(SixnetDataCommand command, Dictionary<string, dynamic> identities)
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
                await ExecuteDataCommandCallbackAsync(command).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Hanle queued execution callback
        /// </summary>
        async Task HandleQueuedExecutionCallbackAsync(CancellationToken cancellationToken = default)
        {
            var executionTasks = new List<Task>();
            while (executedCommands.TryDequeue(out var command))
            {
                executionTasks.Add(ExecuteDataCommandCallbackAsync(command, cancellationToken));
            }
            await Task.WhenAll(executionTasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute data command callback
        /// </summary>
        /// <param name="command">Command</param>
        async Task ExecuteDataCommandCallbackAsync(SixnetDataCommand command, CancellationToken cancellationToken = default)
        {
            // callback data event
            await SixnetDataEventBus.PublishExecutedDataEvent(this, command, cancellationToken).ConfigureAwait(false);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        #endregion
    }
}
