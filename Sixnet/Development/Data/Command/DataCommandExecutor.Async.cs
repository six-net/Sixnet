using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Command
{
    /// <summary>
    /// Data command executor
    /// </summary>
    internal partial class DataCommandExecutor
    {
        #region Query

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Data list</returns>
        public static async Task<List<T>> QueryAsync<T>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<T>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryAsync<T>(GetDatabaseSingleCommand<DatabaseSingleCommand>(conn, queryCommand, options));
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<T>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<T>(0);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Data list</returns>
        public static async Task<T> QueryFirstAsync<T>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            queryCommand?.Queryable?.Take(1, queryCommand?.Queryable?.SkipCount ?? 0);
            T data = default;
            foreach (var conn in connections)
            {
                data = await conn.DatabaseProvider.QueryFirstAsync<T>(GetDatabaseSingleCommand<DatabaseSingleCommand>(conn, queryCommand, options)).ConfigureAwait(false);
                if (data != null)
                {
                    break;
                }
            }
            return data;
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Dynamic object paging</returns>
        public static async Task<PagingInfo<T>> QueryPagingAsync<T>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var pagingFilter = queryCommand?.PagingFilter ?? new PagingFilter();
            queryCommand.PagingFilter = pagingFilter;

            // single conn
            if (connections.GetCount() == 1)
            {
                return await SingleServerPagingAsync<T>(connections.FirstOrDefault(), queryCommand, options).ConfigureAwait(false);
            }

            // mult conn
            var pageSize = pagingFilter.PageSize;
            var page = pagingFilter.Page;
            queryCommand.PagingFilter = PagingFilter.Create(1, page * pageSize);
            var pagingTasks = new Task<PagingInfo<T>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                pagingTasks[taskIndex] = SingleServerPagingAsync<T>(conn, queryCommand, options);
                taskIndex++;
            }
            var allPagingDatas = await Task.WhenAll(pagingTasks).ConfigureAwait(false);
            IEnumerable<T> finallyDatas = Array.Empty<T>();
            var totalCount = 0;
            foreach (var pagingInfo in allPagingDatas)
            {
                if (pagingInfo != null)
                {
                    totalCount += pagingInfo.TotalCount;
                    if (!pagingInfo.Items.IsNullOrEmpty())
                    {
                        finallyDatas = finallyDatas.Union(pagingInfo.Items);
                    };
                }
            }
            if (finallyDatas.GetCount() > pageSize)
            {
                finallyDatas = finallyDatas.Skip((page - 1) * pageSize).Take(pageSize);
            }
            queryCommand.PagingFilter = pagingFilter;
            return Pager.Create(page, pageSize, totalCount, finallyDatas);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="connection">Database </param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Return data paging</returns>
        static async Task<PagingInfo<T>> SingleServerPagingAsync<T>(DatabaseConnection connection, DataCommand queryCommand, DataOperationOptions options = null)
        {
            var provider = connection.DatabaseProvider;
            return await provider.QueryPagingAsync<T>(GetDatabaseSingleCommand<DatabaseSingleCommand>(connection, queryCommand, options)).ConfigureAwait(false);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
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
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
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
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
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
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<List<TReturn>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMappingAsync(databaseCommand);
                taskIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false))?.SelectMany(c => c) ?? new List<TReturn>(0);
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Whether exists data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public static async Task<bool> ExistsAsync(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<bool>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                queryTasks[taskIndex] = conn.DatabaseProvider.ExistsAsync(GetDatabaseSingleCommand<DatabaseSingleCommand>(conn, queryCommand, options));
                taskIndex++;
            }
            var datas = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            return datas.Any(c => c);
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Data count</returns>
        public static async Task<int> CountAsync(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var countTasks = new List<Task<int>>();
            foreach (var conn in connections)
            {
                countTasks.Add(conn.DatabaseProvider.CountAsync(GetDatabaseSingleCommand<DatabaseSingleCommand>(conn, queryCommand, options)));
            }
            return (await Task.WhenAll(countTasks).ConfigureAwait(false)).Sum();
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public static async Task<TValue> ScalarAsync<TValue>(IEnumerable<DatabaseConnection> connections, DataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var scalarTasks = new Task<TValue>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                scalarTasks[taskIndex] = conn.DatabaseProvider.ScalarAsync<TValue>(GetDatabaseSingleCommand<DatabaseSingleCommand>(conn, queryCommand, options));
                taskIndex++;
            }
            var values = await Task.WhenAll(scalarTasks).ConfigureAwait(false);
            var conversionName = queryCommand?.Queryable?.SelectedFields?.FirstOrDefault()?.FormatOption?.Name;
            dynamic result = conversionName switch
            {
                FieldFormatterNames.MAX => values.Max(),
                FieldFormatterNames.MIN => values.Min(),
                FieldFormatterNames.SUM or FieldFormatterNames.COUNT => Sum(values),
                FieldFormatterNames.AVG => Average(values),
                _ => values.FirstOrDefault(),
            };
            return result;
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        public static async Task<DataSet> QueryMultipleAsync(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<DataSet>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync(databaseCommand);
                taskIndex++;
            }
            var finallyDataSet = new DataSet();
            var dataSets = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var valueDataSet in dataSets)
            {
                if ((valueDataSet?.Tables?.Count ?? 0) < 1)
                {
                    continue;
                }
                foreach (DataTable table in valueDataSet.Tables)
                {
                    finallyDataSet.Tables.Add(table);
                }
            }
            return finallyDataSet;
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>>(firstDatas, secondDatas);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond, TThird>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
                if (!(data?.Item3.IsNullOrEmpty() ?? true))
                {
                    thirdDatas.AddRange(data.Item3);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>>(firstDatas, secondDatas, thirdDatas);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
                if (!(data?.Item3.IsNullOrEmpty() ?? true))
                {
                    thirdDatas.AddRange(data.Item3);
                }
                if (!(data?.Item4.IsNullOrEmpty() ?? true))
                {
                    fourthDatas.AddRange(data.Item4);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>(firstDatas, secondDatas, thirdDatas, fourthDatas);
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
                if (!(data?.Item3.IsNullOrEmpty() ?? true))
                {
                    thirdDatas.AddRange(data.Item3);
                }
                if (!(data?.Item4.IsNullOrEmpty() ?? true))
                {
                    fourthDatas.AddRange(data.Item4);
                }
                if (!(data?.Item5.IsNullOrEmpty() ?? true))
                {
                    fifthDatas.AddRange(data.Item5);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas);
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
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
            var sixthDatas = new List<TSixth>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
                if (!(data?.Item3.IsNullOrEmpty() ?? true))
                {
                    thirdDatas.AddRange(data.Item3);
                }
                if (!(data?.Item4.IsNullOrEmpty() ?? true))
                {
                    fourthDatas.AddRange(data.Item4);
                }
                if (!(data?.Item5.IsNullOrEmpty() ?? true))
                {
                    fifthDatas.AddRange(data.Item5);
                }
                if (!(data?.Item6.IsNullOrEmpty() ?? true))
                {
                    sixthDatas.AddRange(data.Item6);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas, sixthDatas);
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
        /// <param name="connections">Connections</param>
        /// <param name="queryCommands">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var queryTasks = new Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                queryTasks[taskIndex] = conn.DatabaseProvider.QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(databaseCommand);
                taskIndex++;
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
            var sixthDatas = new List<TSixth>();
            var seventhDatas = new List<TSeventh>();
            var dataCollection = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var data in dataCollection)
            {
                if (!(data?.Item1.IsNullOrEmpty() ?? true))
                {
                    firstDatas.AddRange(data.Item1);
                }
                if (!(data?.Item2.IsNullOrEmpty() ?? true))
                {
                    secondDatas.AddRange(data.Item2);
                }
                if (!(data?.Item3.IsNullOrEmpty() ?? true))
                {
                    thirdDatas.AddRange(data.Item3);
                }
                if (!(data?.Item4.IsNullOrEmpty() ?? true))
                {
                    fourthDatas.AddRange(data.Item4);
                }
                if (!(data?.Item5.IsNullOrEmpty() ?? true))
                {
                    fifthDatas.AddRange(data.Item5);
                }
                if (!(data?.Item6.IsNullOrEmpty() ?? true))
                {
                    sixthDatas.AddRange(data.Item6);
                }
                if (!(data?.Item7.IsNullOrEmpty() ?? true))
                {
                    seventhDatas.AddRange(data.Item7);
                }
            }
            return new Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>(firstDatas, secondDatas, thirdDatas, fourthDatas, fifthDatas, sixthDatas, seventhDatas);
        }

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static Task<List<DatabaseTableInfo>> GetTablesAsync(DatabaseConnection connection, DataOperationOptions options = null)
        {
            return connection.DatabaseProvider.GetTablesAsync(new DatabaseCommand()
            {
                Connection = connection,
                CancellationToken = options?.CancellationToken ?? default
            });
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> commands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            // single connection
            if (connections.GetCount() == 1)
            {
                var firstConnection = connections.First();
                var databaseCommand = GetDatabaseMultipleCommand(firstConnection, commands, options);
                return await firstConnection.DatabaseProvider.ExecuteAsync(databaseCommand).ConfigureAwait(false);
            }
            else // multiple connection
            {
                var executionTasks = new Task<int>[connections.GetCount()];
                var taskIndex = 0;
                foreach (var conn in connections)
                {
                    executionTasks[taskIndex] = conn.DatabaseProvider.ExecuteAsync(GetDatabaseMultipleCommand(conn, commands, options));
                    taskIndex++;
                }
                var values = await Task.WhenAll(executionTasks).ConfigureAwait(false);
                return values.Sum(c => c);
            }
        }

        /// <summary>
        /// Insert data and return auto Identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns>Inserted data identities,Key: command id, Value: identity value</returns>
        public static async Task<Dictionary<string, TIdentity>> InsertAndReturnAutoIdentityAsync<TIdentity>(IEnumerable<DatabaseConnection> connections, IEnumerable<DataCommand> commands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            // single connection
            if (connections.GetCount() == 1)
            {
                var firstConnection = connections.First();
                var databaseCommand = GetDatabaseMultipleCommand(firstConnection, commands, options);
                return await firstConnection.DatabaseProvider.InsertAndReturnIdentityAsync<TIdentity>(databaseCommand).ConfigureAwait(false);
            }
            else // multiple connection
            {
                var insertTasks = new Task<Dictionary<string, TIdentity>>[connections.GetCount()];
                var taskIndex = 0;
                foreach (var conn in connections)
                {
                    insertTasks[taskIndex] = conn.DatabaseProvider.InsertAndReturnIdentityAsync<TIdentity>(GetDatabaseMultipleCommand(conn, commands, options));
                    taskIndex++;
                }
                return (await Task.WhenAll(insertTasks).ConfigureAwait(false))
                    .SelectMany(c => c)
                    .GroupBy(c => c.Key, c => c.Value)
                    .ToDictionary(c => c.Key, c => c.FirstOrDefault());
            }
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static async Task BulkInsertAsync(IEnumerable<DatabaseConnection> connections, DataTable dataTable, IBulkInsertionOptions options = null)
        {
            ValidateConnections(connections);
            var insertTasks = new Task[connections.GetCount()];
            var taskIndex = 0;
            foreach (var conn in connections)
            {
                insertTasks[taskIndex] = conn.DatabaseProvider.BulkInsertAsync(GetDatabaseBulkInsertCommand(conn, dataTable, options));
                taskIndex++;
            }
            await Task.WhenAll(insertTasks).ConfigureAwait(false);
        }

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public static Task MigrateAsync(IEnumerable<DatabaseConnection> connections, MigrationInfo migrationInfo, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var migrationTasks = new List<Task>();
            foreach (var connection in connections)
            {
                migrationTasks.Add(connection.DatabaseProvider.MigrateAsync(new DatabaseMigrationCommand()
                {
                    CancellationToken = options?.CancellationToken,
                    Connection = connection,
                    MigrationInfo = migrationInfo
                }));
            }
            return Task.WhenAll(migrationTasks);
        }

        #endregion
    }
}
