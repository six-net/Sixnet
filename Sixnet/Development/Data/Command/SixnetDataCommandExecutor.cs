// "Company © 2025. All rights reserved."

using System.Data;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Options;

using Sixnet.Development.Data;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Model.Paging;

using static Sixnet.Reflection.SixnetReflecter;

namespace Sixnet.Development.Command
{
    /// <summary>
    /// Data command executor
    /// </summary>
    internal partial class SixnetDataCommandExecutor
    {
        #region Query

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Data list</returns>
        public static List<T> Query<T>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<T>();
            foreach (var conn in connections)
            {
                var connDatas = conn.DatabaseProvider.Query<T>(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(conn, queryCommand, options));
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<T>(0);
        }

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Data list</returns>
        public static T QueryFirst<T>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            queryCommand?.Queryable?.Take(1, queryCommand?.Queryable?.SkipCount ?? 0);
            T data = default;
            foreach (var conn in connections)
            {
                data = conn.DatabaseProvider.QueryFirst<T>(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(conn, queryCommand, options));
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
        public static SixnetPagingInfo<T> QueryPaging<T>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var pagingFilter = queryCommand?.PagingFilter ?? new SixnetPagingFilter();
            queryCommand.PagingFilter = pagingFilter;

            // single connection
            if (connections.GetCount() == 1)
            {
                return SingleServerPaging<T>(connections.FirstOrDefault(), queryCommand, options);
            }

            // mult connection
            var pageSize = pagingFilter.PageSize;
            var page = pagingFilter.Page;
            queryCommand.PagingFilter = SixnetPagingFilter.Create(1, page * pageSize);
            var allPagings = new List<SixnetPagingInfo<T>>();
            foreach (var conn in connections)
            {
                allPagings.Add(SingleServerPaging<T>(conn, queryCommand, options));
            }
            IEnumerable<T> finallyDatas = Array.Empty<T>();
            var totalCount = 0;
            foreach (var pagingInfo in allPagings)
            {
                if (pagingInfo != null)
                {
                    totalCount += pagingInfo.TotalCount;
                    if (!pagingInfo.Items.IsNullOrEmpty())
                    {
                        finallyDatas = finallyDatas.Union(pagingInfo.Items);
                    }
                    ;
                }
            }
            if (finallyDatas.GetCount() > pageSize)
            {
                finallyDatas = finallyDatas.Skip((page - 1) * pageSize).Take(pageSize);
            }
            queryCommand.PagingFilter = pagingFilter;
            return SixnetPager.Create(page, pageSize, totalCount, finallyDatas);
        }

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="connection">Database </param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Data operation options</param>
        /// <returns>Return data paging</returns>
        static SixnetPagingInfo<T> SingleServerPaging<T>(SixnetDatabaseConnection connection, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            var provider = connection.DatabaseProvider;
            return provider.QueryPaging<T>(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(connection, queryCommand, options));
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<SixnetQueryMappingDatabaseCommand<TFirst, TSecond, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>>(conn, queryCommand, options);
                databaseCommand.DataMappingFunc = dataMappingFunc;
                var connDatas = conn.DatabaseProvider.QueryMapping(databaseCommand);
                if (!connDatas.IsNullOrEmpty())
                {
                    datas.AddRange(connDatas);
                }
            }
            return HandleMultipleSourceDatas(datas, queryCommand?.Queryable)?.ToList() ?? new List<TReturn>(0);
        }

        /// <summary>
        /// Whether exists data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        public static bool Exists(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var conn in connections)
            {
                if (conn.DatabaseProvider.Exists(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(conn, queryCommand, options)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Data count</returns>
        public static int Count(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var count = 0;
            foreach (var conn in connections)
            {
                count += conn.DatabaseProvider.Count(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(conn, queryCommand, options));
            }
            return count;
        }

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="queryCommand">Query data command</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        public static TValue Scalar<TValue>(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataCommand queryCommand, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var values = new List<TValue>();
            foreach (var conn in connections)
            {
                values.Add(conn.DatabaseProvider.Scalar<TValue>(GetDatabaseSingleCommand<SixnetSingleDatabaseCommand>(conn, queryCommand, options)));
            }
            var conversionName = queryCommand?.Queryable?.SelectedFields?.FirstOrDefault()?.FormatSetting?.Name;
            dynamic result = conversionName switch
            {
                SixnetFieldFormatterNames.MAX => values.Max(),
                SixnetFieldFormatterNames.MIN => values.Min(),
                SixnetFieldFormatterNames.SUM or SixnetFieldFormatterNames.COUNT => Sum(values),
                SixnetFieldFormatterNames.AVG => Average(values),
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
        public static DataSet QueryMultiple(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataSets = new List<DataSet>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataSets.Add(conn.DatabaseProvider.QueryMultiple(databaseCommand));
            }
            if (dataSets.IsNullOrEmpty())
            {
                return null;
            }
            if (dataSets.Count == 1)
            {
                return dataSets.FirstOrDefault();
            }
            var finallyDataSet = new DataSet();
            foreach (var valueDataSet in dataSets)
            {
                if ((valueDataSet?.Tables?.Count ?? 0) < 1)
                {
                    continue;
                }
                while (valueDataSet.Tables.Count > 0)
                {
                    var firstTable = valueDataSet.Tables[0];
                    valueDataSet.Tables.Remove(firstTable);
                    finallyDataSet.Tables.Add(firstTable);
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
        public static Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond, TThird>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond, TThird, TFourth>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
            var sixthDatas = new List<TSixth>();
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataCollection = new List<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataCollection.Add(conn.DatabaseProvider.QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(databaseCommand));
            }
            var firstDatas = new List<TFirst>();
            var secondDatas = new List<TSecond>();
            var thirdDatas = new List<TThird>();
            var fourthDatas = new List<TFourth>();
            var fifthDatas = new List<TFifth>();
            var sixthDatas = new List<TSixth>();
            var seventhDatas = new List<TSeventh>();
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
        public static List<SixnetDataTable> GetTables(SixnetDatabaseConnection connection, SixnetDataOperationOptions options = null)
        {
            return connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options));
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
        public static int Execute(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> commands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var value = 0;
            foreach (var conn in connections)
            {
                value += conn.DatabaseProvider.Execute(GetDatabaseMultipleCommand(conn, commands, options));
            }
            return value;
        }

        /// <summary>
        /// Insert data and return auto Identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity data type</typeparam>
        /// <param name="connections">Connections</param>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns>Inserted data identities,Key: command id, Value: identity value</returns>
        public static Dictionary<string, TIdentity> InsertAndReturnAutoIdentity<TIdentity>(IEnumerable<SixnetDatabaseConnection> connections, IEnumerable<SixnetDataCommand> commands, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var identityCollection = new List<Dictionary<string, TIdentity>>();
            foreach (var conn in connections)
            {
                var connIdentities = conn.DatabaseProvider.InsertAndReturnIdentity<TIdentity>(GetDatabaseMultipleCommand(conn, commands, options));
                if (!connIdentities.IsNullOrEmpty())
                {
                    identityCollection.Add(connIdentities);
                }
            }
            return identityCollection
                .SelectMany(c => c)
                .GroupBy(c => c.Key, c => c.Value)
                .ToDictionary(c => c.Key, c => c.FirstOrDefault());
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static void BulkInsert(IEnumerable<SixnetDatabaseConnection> connections, DataTable dataTable, ISixnetBulkInsertionOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var conn in connections)
            {
                conn.DatabaseProvider.BulkInsert(GetDatabaseBulkInsertCommand(conn, dataTable, options));
            }
        }

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        public static void Migrate(IEnumerable<SixnetDatabaseConnection> connections, SixnetMigrationInfo migrationInfo, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                var command = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
                {
                    cmd.MigrationInfo = migrationInfo;
                });
                connection.DatabaseProvider.Migrate(command);
            }
        }

        /// <summary>
        /// Get create table command
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetCreateTableCommand(SixnetDatabaseConnection connection, Type entityType, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo()
                {
                    NewTables =
                    [
                        new SixnetNewTableInfo()
                        {
                            EntityType = entityType,
                        }
                    ]
                };
            });
            var rootTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig);
            List<SixnetDatabaseObjectName> tableNames = null;
            if (entityConfig.IsSplitTable)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                tableNames = splitProvider.ResolveTableNames(new SixnetResolveSplitTableNameParameter()
                {
                    SplitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior()
                    {
                        SelectionPattern = SixnetSplitTableNameSelectionPattern.Range,
                        SplitValues = entityConfig.SplitTableType != SixnetSplitTableType.Custom ? new List<dynamic>(1) { DateTimeOffset.Now } : null
                    },
                    RootTableName = rootTableName,
                    EntityConfiguration = entityConfig,
                    ExpansionNum = entityConfig.AutoExpansionSplitNum
                });
            }
            else
            {
                tableNames = [rootTableName];
            }
            migCmd.MigrationInfo.NewTables[0].TableNames = tableNames;
            return migCmd;
        }

        /// <summary>
        /// Get delete table name
        /// </summary>
        /// <param name="rootTableName"></param>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetDeleteTableCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo();
            });
            var rootTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig);
            List<SixnetDatabaseObjectName> tableNames = null;
            if (entityConfig.IsSplitTable)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                allTableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = rootTableName,
                });
                var splitTableNames = splitProvider.ResolveTableNames(new SixnetResolveSplitTableNameParameter()
                {
                    EntityConfiguration = entityConfig,
                    RootTableName = rootTableName,
                    SplitBehavior = splitBehavior
                });
                if (options?.SplitTableBehavior.IsTakeAllSplitTables(splitTableNames) ?? true)
                {
                    tableNames = allTableNames;
                }
                else
                {
                    tableNames = splitProvider.GetTableNames(new SixnetGetSplitTableNameParameter()
                    {
                        AllTableNames = allTableNames,
                        RootTableName = rootTableName,
                        ResolvedTableNames = splitTableNames,
                        Behavior = splitBehavior
                    });
                }
            }
            else
            {
                tableNames = [rootTableName];
            }
            migCmd.MigrationInfo.DeletedTables = tableNames;
            return migCmd;
        }

        /// <summary>
        /// Get add field command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="allTableNames"></param>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetAddFieldCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo();
            });
            var rootTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig);
            List<SixnetDatabaseObjectName> tableNames = null;
            if (entityConfig.IsSplitTable)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                tableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    RootTableName = rootTableName,
                    Behavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior()
                });
            }
            else
            {
                tableNames = [rootTableName];
            }
            var newFieldDict = new Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>>();
            foreach (var tableName in tableNames)
            {
                newFieldDict[tableName] = fields;
            }
            migCmd.MigrationInfo.NewFields = newFieldDict;
            return migCmd;
        }

        /// <summary>
        /// Get delete field command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="allTableNames"></param>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetDeleteFieldCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo();
            });
            var rootTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig);
            List<SixnetDatabaseObjectName> tableNames = null;
            if (entityConfig.IsSplitTable)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                tableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    RootTableName = rootTableName,
                    Behavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior()
                });
            }
            else
            {
                tableNames = [rootTableName];
            }
            var tableFieldDict = new Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>>();
            foreach (var tableName in tableNames)
            {
                tableFieldDict[tableName] = fields;
            }
            migCmd.MigrationInfo.DeletedFields = tableFieldDict;
            return migCmd;
        }

        /// <summary>
        /// Get alter field command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="allTableNames"></param>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetAlterFieldCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, Dictionary<string, SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo();
            });
            var rootTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig);
            List<SixnetDatabaseObjectName> tableNames = null;
            if (entityConfig.IsSplitTable)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                tableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    RootTableName = rootTableName,
                    Behavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior()
                });
            }
            else
            {
                tableNames = [rootTableName];
            }
            var tableFieldDict = new Dictionary<SixnetDatabaseObjectName, Dictionary<string, SixnetDataField>>();
            foreach (var tableName in tableNames)
            {
                tableFieldDict[tableName] = fields;
            }
            migCmd.MigrationInfo.UpdatedFields = tableFieldDict;
            return migCmd;
        }

        /// <summary>
        /// Get rename table command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityType"></param>
        /// <param name="allTableNames"></param>
        /// <param name="newTableName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static SixnetMigrationDatabaseCommand GetRenameTableCommand(SixnetDatabaseConnection connection, Type entityType
            , List<SixnetDatabaseObjectName> allTableNames, SixnetDatabaseObjectName oldTableName, SixnetDatabaseObjectName newTableName
            , SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo();
            });
            if (string.IsNullOrWhiteSpace(newTableName.Name))
            {
                newTableName.Name = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig).Name;
            }
            if (entityConfig?.IsSplitTable ?? false)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), SixnetEntityManager.GetEntityConfig(entityType));
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                allTableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = oldTableName,
                });
                migCmd.MigrationInfo.RenamedTables = splitProvider.ChangeRootTableNames(allTableNames, newTableName);
            }
            else
            {
                migCmd.MigrationInfo.RenamedTables = new Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName>
                {
                    { oldTableName, newTableName}
                };
            }
            return migCmd;
        }

        static SixnetMigrationDatabaseCommand GetAddForeignKeyCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type selfEntityType, string selfField, Type referenceEntityType, string referenceField, SixnetDataOperationOptions options = null)
        {
            var selfEntityConfig = SixnetEntityManager.GetEntityConfig(selfEntityType);
            var selfDataField = SixnetEntityManager.GetField(selfEntityType, selfField);
            var selfFieldDatabaseObject = SixnetDatabaseObjectName.Create(selfDataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
            var referenceEntityConfig = SixnetEntityManager.GetEntityConfig(referenceEntityType);
            var referenceDataField = SixnetEntityManager.GetField(referenceEntityType, referenceField);
            var referenceFieldDatabaseObject = SixnetDatabaseObjectName.Create(referenceDataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo()
                {
                    NewForeignKeys = []
                };
            });
            var selfTableNames = new List<SixnetDatabaseObjectName>() { SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), selfEntityConfig) };
            if (selfEntityConfig?.IsSplitTable ?? false)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), selfEntityConfig);
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                selfTableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = selfTableNames.FirstOrDefault(),
                });
            }
            var referenceTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), referenceEntityConfig);
            foreach (var selfTable in selfTableNames)
            {
                migCmd.MigrationInfo.NewForeignKeys.Add(new SixnetEntityForeignKeyInfo()
                {
                    SourceTable = selfTable,
                    SourceField = selfFieldDatabaseObject,
                    ReferenceTable = referenceTableName,
                    ReferenceField = referenceFieldDatabaseObject
                });
            }
            return migCmd;
        }

        static SixnetMigrationDatabaseCommand GetDeleteForeignKeyCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type selfEntityType, string selfField, Type referenceEntityType, string referenceField, SixnetDataOperationOptions options = null)
        {
            var selfEntityConfig = SixnetEntityManager.GetEntityConfig(selfEntityType);
            var selfDataField = SixnetEntityManager.GetField(selfEntityType, selfField);
            var selfFieldDatabaseObject = SixnetDatabaseObjectName.Create(selfDataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
            var referenceEntityConfig = SixnetEntityManager.GetEntityConfig(referenceEntityType);
            var referenceDataField = SixnetEntityManager.GetField(referenceEntityType, referenceField);
            var referenceFieldDatabaseObject = SixnetDatabaseObjectName.Create(referenceDataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo()
                {
                    DeletedForeignKeys = []
                };
            });
            var selfTableNames = new List<SixnetDatabaseObjectName>() { SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), selfEntityConfig) };
            if (selfEntityConfig?.IsSplitTable ?? false)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), selfEntityConfig);
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                selfTableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = selfTableNames.FirstOrDefault(),
                });
            }
            var referenceTableName = SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), referenceEntityConfig);
            foreach (var selfTable in selfTableNames)
            {
                migCmd.MigrationInfo.DeletedForeignKeys.Add(new SixnetEntityForeignKeyInfo()
                {
                    SourceTable = selfTable,
                    SourceField = selfFieldDatabaseObject,
                    ReferenceTable = referenceTableName,
                    ReferenceField = referenceFieldDatabaseObject
                });
            }
            return migCmd;
        }

        static SixnetMigrationDatabaseCommand GetAddIndexCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, bool unique, IEnumerable<string> fields, List<SixnetEntityIndexField> indexFields = null, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo()
                {
                    NewIndexes = []
                };
            });

            var tableNames = new List<SixnetDatabaseObjectName>() { SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig) };
            if (entityConfig?.IsSplitTable ?? false)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), entityConfig);
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                tableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = tableNames.FirstOrDefault(),
                });
            }

            if (indexFields.IsNullOrEmpty() && !fields.IsNullOrEmpty())
            {
                indexFields = new List<SixnetEntityIndexField>();
                foreach (var field in fields)
                {
                    var dataField = SixnetEntityManager.GetField(entityType, field);
                    var dataFieldObject = SixnetDatabaseObjectName.Create(dataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
                    indexFields.Add(new SixnetEntityIndexField()
                    {
                        Name = dataFieldObject,
                        Desc = dataField.HasDbFeature(SixnetFieldDbFeature.IndexDesc),
                        Sequence = dataField.IndexSequence
                    });
                }
            }

            foreach (var table in tableNames)
            {
                migCmd.MigrationInfo.NewIndexes.Add(new SixnetEntityIndexInfo()
                {
                    Table = table,
                    Unique = unique,
                    Fields = indexFields
                });
            }

            return migCmd;
        }

        static SixnetMigrationDatabaseCommand GetDeleteIndexCommand(SixnetDatabaseConnection connection, List<SixnetDatabaseObjectName> allTableNames, Type entityType, bool unique, IEnumerable<string> fields, List<SixnetEntityIndexField> indexFields = null, SixnetDataOperationOptions options = null)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
            {
                cmd.MigrationInfo = new SixnetMigrationInfo()
                {
                    DeletedIndexes = []
                };
            });

            var tableNames = new List<SixnetDatabaseObjectName>() { SixnetDataManager.GetDefaultTableName(SixnetDataCommandExecutionContext.Create(connection), entityConfig) };
            if (entityConfig?.IsSplitTable ?? false)
            {
                var splitProvider = SixnetDataManager.GetSplitTableProvider(SixnetDataManager.GetDataOptions(), entityConfig);
                var splitBehavior = options?.SplitTableBehavior ?? new SixnetSplitTableBehavior();
                tableNames = splitProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
                {
                    AllTableNames = allTableNames,
                    Behavior = splitBehavior,
                    RootTableName = tableNames.FirstOrDefault(),
                });
            }

            if (indexFields.IsNullOrEmpty() && !fields.IsNullOrEmpty())
            {
                indexFields = new List<SixnetEntityIndexField>();
                foreach (var field in fields)
                {
                    var dataField = SixnetEntityManager.GetField(entityType, field);
                    var dataFieldObject = SixnetDatabaseObjectName.Create(dataField.GetFieldName(connection.DatabaseServer.DatabaseType), SixnetDatabaseObjectType.Column);
                    indexFields.Add(new SixnetEntityIndexField()
                    {
                        Name = dataFieldObject,
                        Desc = dataField.HasDbFeature(SixnetFieldDbFeature.IndexDesc),
                        Sequence = dataField.IndexSequence
                    });
                }
            }

            foreach (var table in tableNames)
            {
                migCmd.MigrationInfo.DeletedIndexes.Add(new SixnetEntityIndexInfo()
                {
                    Table = table,
                    Unique = unique,
                    Fields = indexFields
                });
            }

            return migCmd;
        }

        #endregion

        #region Create table

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        public static void CreateTable(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetCreateTableCommand(connection, entityType, options));
            }
        }

        #endregion

        #region Delete table

        /// <summary>
        /// Delete table
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        public static void DeleteTable(IEnumerable<SixnetDatabaseConnection> connections, List<Type> entityTypes, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                foreach (var entityType in entityTypes)
                {
                    var allTableNames = connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList();
                    connection.DatabaseProvider.Migrate(GetDeleteTableCommand(connection, allTableNames, entityType, options));
                }
            }
        }

        #endregion

        #region Add field

        /// <summary>
        /// Add field
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        public static void AddField(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetAddFieldCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList(), entityType, fields, options));
            }
        }

        #endregion

        #region Delete field

        /// <summary>
        /// Delete field
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        public static void DeleteField(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, List<SixnetDataField> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetDeleteFieldCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList(), entityType, fields, options));
            }
        }

        #endregion

        #region Alter field

        /// <summary>
        /// Alter fields
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        public static void AlterField(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, Dictionary<string, SixnetDataField> fields, SixnetDataOperationOptions options)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetAlterFieldCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList(), entityType, fields, options));
            }
        }

        #endregion

        #region Rename table

        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="oldTableName">Old table name</param>
        /// <param name="newTableName">New table name</param>
        /// <param name="options">Options</param>
        public static void RenameTable(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, SixnetDatabaseObjectName oldTableName, SixnetDatabaseObjectName newTableName, SixnetDataOperationOptions options)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetRenameTableCommand(connection, entityType, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList(), oldTableName, newTableName, options));
            }
        }

        #endregion

        #region Add foreign key

        /// <summary>
        /// Add foreign key
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="selfEntityType">Self entity type</param>
        /// <param name="selfField">Self field</param>
        /// <param name="referenceEntityType">Reference entity type</param>
        /// <param name="referenceField">Reference field</param>
        /// <param name="options">Options</param>
        public static void AddForeignKey(IEnumerable<SixnetDatabaseConnection> connections, Type selfEntityType, string selfField, Type referenceEntityType, string referenceField, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetAddForeignKeyCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , selfEntityType, selfField, referenceEntityType, referenceField, options));
            }
        }

        #endregion

        #region Delete foreign key

        /// <summary>
        /// Delete foreign key
        /// </summary>
        /// <param name="connections">Connections</param>
        /// <param name="selfEntityType">Self entity type</param>
        /// <param name="selfField">Self field</param>
        /// <param name="referenceEntityType">Reference entity type</param>
        /// <param name="referenceField">Reference field</param>
        /// <param name="options">Options</param>
        public static void DeleteForeignKey(IEnumerable<SixnetDatabaseConnection> connections, Type selfEntityType, string selfField, Type referenceEntityType, string referenceField, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetDeleteForeignKeyCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , selfEntityType, selfField, referenceEntityType, referenceField, options));
            }
        }

        /// <summary>
        /// Delete all foreign keys
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="options"></param>
        public static void DeleteAllForeignKeys(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
                {
                    cmd.MigrationInfo = new SixnetMigrationInfo()
                    {
                        DeleteAllForeignKey = true
                    };
                });
                connection.DatabaseProvider.Migrate(migCmd);
            }
        }

        #endregion

        #region Add index

        /// <summary>
        /// Add index
        /// </summary>
        /// <param name="unique">Whether is unique index</param>
        /// <param name="fields">Fields</param>
        public static void AddIndex(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, bool unique, IEnumerable<string> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetAddIndexCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , entityType, unique, fields, null, options));
            }
        }

        /// <summary>
        /// Add index
        /// </summary>
        /// <param name="unique">Whether is unique index</param>
        /// <param name="fields">Fields</param>
        public static void AddIndex(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, bool unique, List<SixnetEntityIndexField> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetAddIndexCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , entityType, unique, null, fields, options));
            }
        }

        #endregion

        #region Delete index

        /// <summary>
        /// Delete index
        /// </summary>
        /// <param name="fields">Fields</param>
        public static void DeleteIndex(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, IEnumerable<string> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetDeleteIndexCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , entityType, false, fields, null, options));
            }
        }

        /// <summary>
        /// Delete index
        /// </summary>
        /// <param name="fields">Fields</param>
        public static void DeleteIndex(IEnumerable<SixnetDatabaseConnection> connections, Type entityType, List<SixnetEntityIndexField> fields, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(GetDeleteIndexCommand(connection, connection.DatabaseProvider.GetTables(SixnetDatabaseCommand.Create(connection, options))?.Select(c => c.GetDatabaseObjectName()).ToList()
                    , entityType, false, null, fields, options));
            }
        }

        #endregion

        #region Clear database

        /// <summary>
        /// Clear database
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="options"></param>
        public static void ClearDatabase(IEnumerable<SixnetDatabaseConnection> connections, SixnetDataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                var migCmd = SixnetDatabaseCommand.Create<SixnetMigrationDatabaseCommand>(connection, options, cmd =>
                {
                    cmd.MigrationInfo = new SixnetMigrationInfo()
                    {
                        ClearDatabase = true
                    };
                });
                connection.DatabaseProvider.Migrate(migCmd);
            }
        }

        #endregion

        #region Util

        /// <summary>
        /// Get a database single command
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        static TDatabaseCommand GetDatabaseSingleCommand<TDatabaseCommand>(SixnetDatabaseConnection connection, SixnetDataCommand dataCommand, SixnetDataOperationOptions options) where TDatabaseCommand : SixnetSingleDatabaseCommand, new()
        {
            var cmd = SixnetDatabaseCommand.Create<TDatabaseCommand>(connection, options, cmd =>
            {
                cmd.DataCommand = dataCommand;
            });

            if (cmd is BaseDatabaseQueryMappingCommand mappingCmd)
            {
                if (!string.IsNullOrWhiteSpace(options?.SpiltOnFieldName))
                {
                    mappingCmd.SpiltOnFieldName = options.SpiltOnFieldName;
                }
            }

            return cmd;
        }

        /// <summary>
        /// Get database multiple command
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="commands">Commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        static SixnetMultipleDatabaseCommand GetDatabaseMultipleCommand(SixnetDatabaseConnection connection, IEnumerable<SixnetDataCommand> commands, SixnetDataOperationOptions options)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), $"{nameof(commands)} is null or empty");

            return SixnetDatabaseCommand.Create<SixnetMultipleDatabaseCommand>(connection, options, cmd =>
            {
                cmd.DataCommands = commands?.ToList() ?? new List<SixnetDataCommand>();
            });
        }

        /// <summary>
        /// Get a database buld insert command
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        static SixnetBulkInsertDatabaseCommand GetDatabaseBulkInsertCommand(SixnetDatabaseConnection connection, DataTable dataTable, ISixnetBulkInsertionOptions options = null)
        {
            return SixnetDatabaseCommand.Create<SixnetBulkInsertDatabaseCommand>(connection, options?.DataOperationOptions, cmd =>
            {
                cmd.BulkInsertionOptions = options;
                cmd.DataTable = dataTable;
            });
        }

        /// <summary>
        /// Validate connections
        /// </summary>
        /// <param name="connections">Database connections</param>
        /// <exception cref="ArgumentNullException"></exception>
        static void ValidateConnections(IEnumerable<SixnetDatabaseConnection> connections)
        {
            SixnetDirectThrower.ThrowArgNullIf(connections.IsNullOrEmpty(), $"{nameof(connections)} is null or empty");
        }

        /// <summary>
        ///  Handle multiple source datas
        /// </summary>
        /// <typeparam name="T">Data source</typeparam>
        /// <param name="originalDatas">Original datas</param>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        static IEnumerable<T> HandleMultipleSourceDatas<T>(IEnumerable<T> originalDatas, ISixnetQueryable queryable)
        {
            if (originalDatas.IsNullOrEmpty())
            {
                return originalDatas;
            }
            var takeCount = queryable?.TakeCount ?? 0;
            if (takeCount > 0)
            {
                originalDatas = originalDatas?.Take(takeCount);
            }
            return originalDatas;
        }

        /// <summary>
        /// Calculate sum
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return sum value</returns>
        static dynamic Sum<T>(IEnumerable<T> datas)
        {
            dynamic result = default(T);
            foreach (dynamic data in datas)
            {
                result += data;
            }
            return result;
        }

        /// <summary>
        /// Calculate average
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return averate value</returns>
        static dynamic Average<T>(IEnumerable<T> datas)
        {
            dynamic result = default(T);
            int count = 0;
            foreach (dynamic data in datas)
            {
                result += data;
                count++;
            }
            return result / count;
        }

        #endregion
    }
}
