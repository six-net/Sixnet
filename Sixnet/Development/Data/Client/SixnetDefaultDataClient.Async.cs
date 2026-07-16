// "Company © 2025. All rights reserved."

using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Sixnet.Development.Command;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Events;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Defines default data client
    /// </summary>
    internal partial class SixnetDefaultDataClient
    {
        #region Query

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return await QueryAsync<T>(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public async Task<List<T>> QueryAsync<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryAsync<T>(connections, command, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return datas;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public Task<List<T>> QueryAsync<T>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryAsync<T>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return await QueryFirstAsync<T>(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public async Task<T> QueryFirstAsync<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var data = await SixnetDataCommandExecutor.QueryFirstAsync<T>(connections, command, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, new List<T>(1) { data }, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return data;
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        public Task<T> QueryFirstAsync<T>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryFirstAsync<T>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<SixnetPagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, SixnetDataOperationOptions options = null)
        {
            return await QueryPagingAsync(conditionExpression, SixnetPagingFilter.Create(page, pageSize), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<SixnetPagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetPagingFilter pagingFilter, SixnetDataOperationOptions options = null)
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
        public async Task<SixnetPagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, int page, int pageSize, SixnetDataOperationOptions options = null)
        {
            return await QueryPagingAsync<T>(queryable, SixnetPagingFilter.Create(page, pageSize), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        public async Task<SixnetPagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, SixnetPagingFilter pagingFilter, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<T>(queryable);
            command.PagingFilter = pagingFilter;
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryPagingAsync<T>(connections, command, options).ConfigureAwait(false);

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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

            // query callback
            await HandleQueryCallbackAsync(command, datas, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

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
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(string script, object parameters, Func<TFirst, TSecond, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand<TReturn>(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var datas = await SixnetDataCommandExecutor.QueryMappingAsync(connections, command, dataMappingFunc, options).ConfigureAwait(false);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMappingAsync(GetScriptQuery(script, parameters, scriptType), dataMappingFunc, options);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return await ExistsAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Query whether has any data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public async Task<bool> ExistsAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var hasData = await SixnetDataCommandExecutor.ExistsAsync(connections, command, options).ConfigureAwait(false);

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
        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return await CountAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> CountAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(GetDataCommandDatabaseServers(command, true, options));
            return await SixnetDataCommandExecutor.CountAsync(connections, command, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MaxAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            var maxQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(SixnetFieldFormatterNames.MAX));
            return await MaxAsync<TValue>(maxQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, SixnetFieldFormatterNames.MAX, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MinAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            var minQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(SixnetFieldFormatterNames.MIN));
            return await MinAsync<TValue>(minQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, SixnetFieldFormatterNames.MIN, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> SumAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            var sumQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(SixnetFieldFormatterNames.SUM));
            return await SumAsync<TValue>(sumQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, SixnetFieldFormatterNames.SUM, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> AvgAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            var avgQueryable = conditionExpression.GetQueryable<T>()
            .Select(field.GetDataField(SixnetFieldFormatterNames.AVG));
            return await AvgAsync<TValue>(avgQueryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            ValidateCalculateField(queryable, SixnetFieldFormatterNames.AVG, options);
            return await ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public async Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            var command = SixnetDataCommand.CreateQueryCommand(queryable);
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, true, options).ConfigureAwait(false));
            var value = await SixnetDataCommandExecutor.ScalarAsync<TValue>(connections, command, options).ConfigureAwait(false);

            // Got value callback
            await HandleGotValueCallbackAsync(command, value, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return value;
        }

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public Task<TValue> ScalarAsync<TValue>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return ScalarAsync<TValue>(GetScriptQuery(script, parameters, scriptType), options);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public async Task<DataSet> QueryMultipleAsync(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<DataSet>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDataSets = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionDataSet(taskDataSets);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public Task<DataSet> QueryMultipleAsync(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
            }
            var taskDatas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return UnionMultipleDatas(taskDatas, taskDatas?.Length ?? 0);
        }


        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond, TThird>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null)
        {
            var commands = queries?.Select(c => SixnetDataCommand.CreateQueryCommand(c));
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, true, options).ConfigureAwait(false);
            var queryTasks = new List<Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>>(serverGroups.Count);
            foreach (var serverItem in serverGroups)
            {
                var serverConnection = GetConnection(serverItem.Value.Item1);
                queryTasks.Add(SixnetDataCommandExecutor.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            return QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(new List<ISixnetQueryable>(1) { GetScriptQuery(script, parameters, scriptType) }, options);
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
        public async Task<int> InsertAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var insertResult = await InsertCoreAsync(datas, options).ConfigureAwait(false);
            return insertResult?.Item1 ?? 0;
        }

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<List<TIdentity>> InsertReturnIdentitiesAsync<T, TIdentity>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var insertResult = await InsertCoreAsync(datas, options).ConfigureAwait(false);
            return insertResult?.Item2?.Values.Cast<TIdentity>().ToList() ?? new List<TIdentity>(0);
        }

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return await InsertAsync<T>(new List<T>(1) { data }, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<TIdentity> InsertReturnIdentityAsync<T, TIdentity>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            var identities = await InsertReturnIdentitiesAsync<T, TIdentity>(new List<T>(1) { data }, options).ConfigureAwait(false);
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
        async Task<Tuple<int, Dictionary<string, dynamic>>> InsertCoreAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
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
                    await data.OnDataAddingAsync().ConfigureAwait(false);

                    SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(T).Name}: {data.GetIdentityValue()} cann't to be add");

                    var addCommand = SixnetDataCommand.Create<T>(SixnetDataOperationType.Insert);
                    var valueDict = data.GetAllValues();
                    addCommand.FieldsAssignment = valueDict?.GetFieldsAssignment();
                    addCommand.Data = data;
                    commands.Add(addCommand);
                }
            }
            var incrementField = SixnetEntityManager.GetField(dataType, SixnetFieldRole.Increment);
            return await ExecuteCoreAsync(commands, incrementField != null, options).ConfigureAwait(false);
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
            var connections = GetConnections(await GetDataCommandDatabaseServersAsync(command, false, options?.DataOperationOptions).ConfigureAwait(false));
            await SixnetDataCommandExecutor.BulkInsertAsync(connections, dataTable, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="datas">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task BulkInsertAsync<T>(IEnumerable<T> datas, ISixnetBulkInsertionOptions options = null)
        {
            return BulkInsertAsync(datas.ToDataTable(), options);
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
        public async Task<int> UpdateAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
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
                    await newData.OnDataUpdatingAsync().ConfigureAwait(false);

                    SixnetException.ThrowIf(!newData.AllowToSave(), $"{typeof(T).Name}: {entityIdentity} cann't to be update");

                    var updateQueryable = SixnetConditionExtensions.IncludeEntity(null, newData);
                    var fieldsAssignment = await newData.GetModificationAssignmentAsync(newData).ConfigureAwait(false);
                    var command = GetUpdateCommand(fieldsAssignment, updateQueryable, options);
                    command.Data = newData;
                    commands.Add(command);
                }
            }
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            return await UpdateAsync(commands).ConfigureAwait(false);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return await UpdateAsync(new List<T>() { data }, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public async Task<int> UpdateAsync<T>(SixnetFieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
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
        public async Task<int> UpdateAsync(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            if (fieldsAssignment == null)
            {
                return 0;
            }

            var command = GetUpdateCommand(fieldsAssignment, queryable, options);
            return await UpdateAsync(new List<SixnetDataCommand>(1) { command }).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="updateCommands">Update commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        async Task<int> UpdateAsync(List<SixnetDataCommand> updateCommands)
        {
            return await ExecuteAsync(updateCommands).ConfigureAwait(false);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            var queryable = SixnetConditionExtensions.IncludeEntities(null, datas);
            var affectedRows = await DeleteAsync(queryable, options).ConfigureAwait(false);
            return affectedRows;
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>
        {
            return await DeleteAsync(new List<T>(1) { data }, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null)
        {
            return await DeleteAsync(conditionExpression.GetQueryable<T>(), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(queryable == null, nameof(queryable));

            var command = SixnetDataCommand.Create(SixnetDataOperationType.Delete, queryable);
            command.Options = options;
            return await ExecuteAsync(new List<SixnetDataCommand>(1) { command }).ConfigureAwait(false);
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
        async Task<Tuple<int, Dictionary<string, dynamic>>> ExecuteCoreAsync(IEnumerable<SixnetDataCommand> commands, bool identityInsert, SixnetDataOperationOptions options = null)
        {
            var affectedRows = 0;
            var serverGroups = await GroupDataCommandsDatabaseServerAsync(commands, false, options).ConfigureAwait(false);
            Dictionary<string, dynamic> identities = null;
            if (identityInsert)
            {
                var executionTasks = new List<Task<Dictionary<string, dynamic>>>(serverGroups.Count);
                foreach (var serverItem in serverGroups)
                {
                    var serverConnection = GetConnection(serverItem.Value.Item1);
                    executionTasks.Add(SixnetDataCommandExecutor.InsertAndReturnAutoIdentityAsync<dynamic>(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
                    executionTasks.Add(SixnetDataCommandExecutor.ExecuteAsync(new List<SixnetDatabaseConnection>(1) { serverConnection }, serverItem.Value.Item2, options));
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
        /// <returns></returns>
        public async Task<int> ExecuteAsync(IEnumerable<SixnetDataCommand> commands)
        {
            return (await ExecuteCoreAsync(commands, false).ConfigureAwait(false)).Item1;
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="command">Data command</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(SixnetDataCommand command)
        {
            return ExecuteAsync(new SixnetDataCommand[1] { command });
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string script, object parameters = null, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, SixnetDataOperationOptions options = null)
        {
            var cmd = SixnetDataCommand.CreateScriptCommand(script, parameters, scriptType);
            cmd.Options = options;
            return await ExecuteAsync(new List<SixnetDataCommand>(1) { cmd }).ConfigureAwait(false);
        }

        #endregion

        #region Migration

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public Task MigrateAsync(SixnetMigrationInfo migrationInfo, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.MigrateAsync(connections, migrationInfo, options);
        }

        /// <summary>
        /// Clear database
        /// </summary>
        /// <param name="options">Data operation options</param>
        public async Task ClearDatabaseAsync(SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            await SixnetDataCommandExecutor.ClearDatabaseAsync(connections, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Create all entity tables
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateAllEntityTablesAsync(SixnetDataOperationOptions options = null)
        {
            var entities = SixnetEntityManager.GetAllEntityConfigs();
            foreach (var entity in entities)
            {
                if (!entity.IsSystem)
                {
                    await CreateTableAsync(entity.EntityType, options);
                }
            }
        }

        /// <summary>
        /// Delete all entity tables
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAllEntityTablesAsync(SixnetDataOperationOptions options = null)
        {
            var connections = GetConnections(internalDatabaseServers);
            await SixnetDataCommandExecutor.DeleteAllForeignKeysAsync(connections, options);

            var entities = SixnetEntityManager.GetAllEntityConfigs();
            await DeleteTableAsync(entities.Where(c => !c.IsSystem).Select(c => c.EntityType).ToList(), options);
        }

        /// <summary>
        /// Create entity table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="options"></param>
        public Task CreateTableAsync<TEntity>(SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity
        {
            return CreateTableAsync(typeof(TEntity), options);
        }

        /// <summary>
        /// Delete entity table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="options"></param>
        public Task DeleteTableAsync<TEntity>(SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity
        {
            return DeleteTableAsync(new List<Type>(1) { typeof(TEntity) }, options);
        }

        /// <summary>
        /// Add field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        public Task AddFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity
        {
            return AddFieldAsync(typeof(TEntity), new List<SixnetDataField>(1) { SixnetExpressionHelper.GetDataField(field) as SixnetDataField }, options);
        }

        /// <summary>
        /// Delete field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public Task DeleteFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity
        {
            return DeleteFieldAsync(typeof(TEntity), new List<SixnetDataField>(1) { SixnetExpressionHelper.GetDataField(field) as SixnetDataField }, options);
        }

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        public Task AlterFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null)
        {
            var dataField = SixnetExpressionHelper.GetDataField(field) as SixnetDataField;
            return AlterFieldAsync(typeof(TEntity), new Dictionary<string, SixnetDataField>(1) { { dataField.FieldName, dataField } }, options);
        }

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        public Task AlterFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, Action<SixnetDataField> configureField, SixnetDataOperationOptions options = null)
        {
            var dataField = SixnetExpressionHelper.GetDataField(field) as SixnetDataField;
            var currentFieldName = dataField.FieldName;
            configureField?.Invoke(dataField);
            return AlterFieldAsync(typeof(TEntity), new Dictionary<string, SixnetDataField>(1) { { currentFieldName, dataField } }, options);
        }

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="field"></param>
        /// <param name="options"></param>
        public Task AlterFieldAsync<TEntity>(string fieldName, SixnetDataField field, SixnetDataOperationOptions options = null)
        {
            return AlterFieldAsync(typeof(TEntity), new Dictionary<string, SixnetDataField>(1) { { fieldName, field } }, options);
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="options">Options</param>
        public Task CreateTableAsync(Type entityType, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.CreateTableAsync(connections, entityType, options);
        }

        /// <summary>
        /// Delete table
        /// </summary>
        /// <param name="entityTypes"></param>
        /// <param name="options"></param>
        public Task DeleteTableAsync(List<Type> entityTypes, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.DeleteTableAsync(connections, entityTypes, options);
        }

        /// <summary>
        /// Add field
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        public Task AddFieldAsync(Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.AddFieldAsync(connections, entityType, fields, options);
        }

        /// <summary>
        /// Delete field
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        public Task DeleteFieldAsync(Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.DeleteFieldAsync(connections, entityType, fields, options);
        }

        /// <summary>
        /// Alter fields
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        public Task AlterFieldAsync(Type entityType, Dictionary<string, SixnetDataField> fields, SixnetDataOperationOptions options)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.AlterFieldAsync(connections, entityType, fields, options);
        }

        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentTableName">Current table name</param>
        /// <param name="newTableName">New table name</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="options">Options</param>
        public Task RenameTableAsync(string currentTableName, string newTableName, Type entityType, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.RenameTableAsync(connections, entityType, SixnetDatabaseObjectName.Create(currentTableName, SixnetDatabaseObjectType.Table)
                , SixnetDatabaseObjectName.Create(newTableName, SixnetDatabaseObjectType.Table), options);
        }

        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentTableName">Current table name</param>
        /// <param name="options">Options</param>
        public Task RenameTableAsync<TEntity>(string currentTableName, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.RenameTableAsync(connections, typeof(TEntity), SixnetDatabaseObjectName.Create(currentTableName, SixnetDatabaseObjectType.Table)
                , SixnetDatabaseObjectName.Create(string.Empty, SixnetDatabaseObjectType.Table), options);
        }

        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentTableName">Current table name</param>
        /// <param name="newTableName">New table name</param>
        /// <param name="schema">Schema</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="options">Options</param>
        public Task RenameTableAsync(string currentTableName, string newTableName, string schema, Type entityType, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.RenameTableAsync(connections, entityType, SixnetDatabaseObjectName.Create(currentTableName, SixnetDatabaseObjectType.Table, schema)
                , SixnetDatabaseObjectName.Create(newTableName, SixnetDatabaseObjectType.Table, schema), options);
        }

        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentTableName">Current table name</param>
        /// <param name="schema">Schema</param>
        /// <param name="options">Options</param>
        public Task RenameTableAsync<TEntity>(string currentTableName, string schema, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.RenameTableAsync(connections, typeof(TEntity), SixnetDatabaseObjectName.Create(currentTableName, SixnetDatabaseObjectType.Table, schema)
                , SixnetDatabaseObjectName.Create(string.Empty, SixnetDatabaseObjectType.Table, schema), options);
        }

        /// <summary>
        /// Add foreign key
        /// </summary>
        /// <typeparam name="TSelfEntity"></typeparam>
        /// <typeparam name="TReferenceEntity"></typeparam>
        /// <param name="selfField">Self field</param>
        /// <param name="referenceField">Reference field</param>
        /// <param name="options">Options</param>
        public Task AddForeignKeyAsync<TSelfEntity, TReferenceEntity>(Expression<Func<TSelfEntity, object>> selfField, Expression<Func<TSelfEntity, object>> referenceField, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.AddForeignKeyAsync(connections, typeof(TSelfEntity), SixnetExpressionHelper.GetDataField(selfField).PropertyName
                , typeof(TReferenceEntity), SixnetExpressionHelper.GetDataField(referenceField).PropertyName, options);
        }

        /// <summary>
        /// Delete foreign key
        /// </summary>
        /// <typeparam name="TSelfEntity"></typeparam>
        /// <typeparam name="TReferenceEntity"></typeparam>
        /// <param name="selfField">Self field</param>
        /// <param name="referenceField">Reference field</param>
        /// <param name="options">Options</param>
        public Task DeleteForeignKeyAsync<TSelfEntity, TReferenceEntity>(Expression<Func<TSelfEntity, object>> selfField, Expression<Func<TSelfEntity, object>> referenceField, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.DeleteForeignKeyAsync(connections, typeof(TSelfEntity), SixnetExpressionHelper.GetDataField(selfField).PropertyName
                , typeof(TReferenceEntity), SixnetExpressionHelper.GetDataField(referenceField).PropertyName, options);
        }

        /// <summary>
        /// Add index
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="unique">Whether is unique index</param>
        /// <param name="fields">Fields</param>
        public Task AddIndexAsync<TEntity>(bool unique, params Expression<Func<TEntity, object>>[] fields)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            var fieldNames = fields?.Select(f => SixnetExpressionHelper.GetDataField(f).PropertyName);
            return SixnetDataCommandExecutor.AddIndexAsync(connections, typeof(TEntity), unique, fieldNames, null);
        }

        /// <summary>
        /// Add index
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="unique">Whether is unique index</param>
        /// <param name="fields">Fields</param>
        public Task AddIndexAsync<TEntity>(bool unique, Func<List<SixnetEntityIndexField>> getIndexFieldsFunc, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.AddIndexAsync(connections, typeof(TEntity), unique, getIndexFieldsFunc?.Invoke(), null);
        }

        /// <summary>
        /// Delete index
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields">Fields</param>
        public Task DeleteIndexAsync<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            var fieldNames = fields?.Select(f => SixnetExpressionHelper.GetDataField(f).PropertyName);
            return SixnetDataCommandExecutor.DeleteIndexAsync(connections, typeof(TEntity), fieldNames, null);
        }

        /// <summary>
        /// Delete index
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="getIndexFieldsFunc">Get index fields func</param>
        /// <param name="options">Options</param>
        public Task DeleteIndexAsync<TEntity>(Func<List<SixnetEntityIndexField>> getIndexFieldsFunc, SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.DeleteIndexAsync(connections, typeof(TEntity), getIndexFieldsFunc?.Invoke(), null);
        }

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        public Task<List<SixnetDataTable>> GetTablesAsync(SixnetDataOperationOptions options = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(internalDatabaseServers.IsNullOrEmpty(), "Database servers");
            var connections = GetConnections(internalDatabaseServers);
            return SixnetDataCommandExecutor.GetTablesAsync(connections?.FirstOrDefault(), options);
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
        async Task<List<SixnetDatabaseServer>> GetDataCommandDatabaseServersAsync(SixnetDataCommand command, bool useForQuery, SixnetDataOperationOptions dataOperationOptions)
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
            await HandleDataCommandBeforeExecutionAsync(command, command.Options?.CancellationToken ?? default).ConfigureAwait(false);

            return servers;
        }

        /// <summary>
        /// Group data commands server
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Key: database server name,Value: commands</returns>
        async Task<Dictionary<string, Tuple<SixnetDatabaseServer, List<SixnetDataCommand>>>> GroupDataCommandsDatabaseServerAsync(IEnumerable<SixnetDataCommand> commands, bool useForQuery, SixnetDataOperationOptions dataOperationOptions)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), nameof(commands));

            var serverGroups = new Dictionary<string, Tuple<SixnetDatabaseServer, List<SixnetDataCommand>>>();
            foreach (var command in commands)
            {
                var databaseServers = await GetDataCommandDatabaseServersAsync(command, useForQuery, dataOperationOptions).ConfigureAwait(false);
                if (!databaseServers.IsNullOrEmpty())
                {
                    foreach (var server in databaseServers)
                    {
                        serverGroups.TryGetValue(server.Name, out var serverCommands);
                        serverCommands ??= new Tuple<SixnetDatabaseServer, List<SixnetDataCommand>>(server, new List<SixnetDataCommand>());
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
            await SixnetEventBus.PublishStartingDataEventAsync(this, dataCommand, true, cancellationToken).ConfigureAwait(false);

            if (dataCommand.ExecutionMode == SixnetCommandExecutionMode.Transform)
            {
                #region Delete

                var queryable = dataCommand.Queryable;
                var options = dataCommand.Options;
                var entityType = dataCommand.GetEntityType();
                if (dataCommand.OperationType == SixnetDataOperationType.Delete && entityType != null && (queryable == null || queryable.ExecutionMode == SixnetQueryableExecutionMode.Regular))
                {
                    #region Logic delete

                    if (SixnetDataManager.AllowLogicalDelete(options))
                    {
                        var archiveFieldName = SixnetEntityManager.GetFieldName(entityType, SixnetFieldRole.Archive);
                        if (!string.IsNullOrWhiteSpace(archiveFieldName))
                        {
                            var fieldsAssignment = SixnetFieldsAssignment.Create();
                            fieldsAssignment.SetNewValue(archiveFieldName, true);
                            dataCommand.OperationType = SixnetDataOperationType.Update;
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
            await SixnetEventBus.PublishQueriedEventAsync(this, command, datas, cancellationToken).ConfigureAwait(false);

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
            await SixnetEventBus.PublishCheckedEventAsync(this, command, hasValue, cancellationToken).ConfigureAwait(false);

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
            await SixnetEventBus.PublishGotValueEventAsync(this, command, value, cancellationToken).ConfigureAwait(false);

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
                case SixnetDataOperationType.Insert:
                case SixnetDataOperationType.Update:
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
            await SixnetEventBus.PublishExecutedDataEventAsync(this, command, cancellationToken).ConfigureAwait(false);

            // command callback event
            SixnetDataManager.TriggerDataCommandCallbackEvent(command);
        }

        #endregion
    }
}
