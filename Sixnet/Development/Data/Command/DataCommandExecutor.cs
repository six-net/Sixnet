using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
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
        public static List<T> Query<T>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<T>();
            foreach (var conn in connections)
            {
                var connDatas = conn.DatabaseProvider.Query<T>(GetDatabaseSingleCommand<SingleDatabaseCommand>(conn, queryCommand, options));
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
        public static T QueryFirst<T>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            queryCommand?.Queryable?.Take(1, queryCommand?.Queryable?.SkipCount ?? 0);
            T data = default;
            foreach (var conn in connections)
            {
                data = conn.DatabaseProvider.QueryFirst<T>(GetDatabaseSingleCommand<SingleDatabaseCommand>(conn, queryCommand, options));
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
        public static PagingInfo<T> QueryPaging<T>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var pagingFilter = queryCommand?.PagingFilter ?? new PagingFilter();
            queryCommand.PagingFilter = pagingFilter;

            // single connection
            if (connections.GetCount() == 1)
            {
                return SingleServerPaging<T>(connections.FirstOrDefault(), queryCommand, options);
            }

            // mult connection
            var pageSize = pagingFilter.PageSize;
            var page = pagingFilter.Page;
            queryCommand.PagingFilter = PagingFilter.Create(1, page * pageSize);
            var allPagings = new List<PagingInfo<T>>();
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
                    };
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
        static PagingInfo<T> SingleServerPaging<T>(DatabaseConnection connection, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            var provider = connection.DatabaseProvider;
            return provider.QueryPaging<T>(GetDatabaseSingleCommand<SingleDatabaseCommand>(connection, queryCommand, options));
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var datas = new List<TReturn>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseSingleCommand<QueryMappingDatabaseCommand<TFirst, TSecond, TReturn>>(conn, queryCommand, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null)
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null)
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null)
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null)
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null)
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
        public static bool Exists(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var conn in connections)
            {
                if (conn.DatabaseProvider.Exists(GetDatabaseSingleCommand<SingleDatabaseCommand>(conn, queryCommand, options)))
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
        public static int Count(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var count = 0;
            foreach (var conn in connections)
            {
                count += conn.DatabaseProvider.Count(GetDatabaseSingleCommand<SingleDatabaseCommand>(conn, queryCommand, options));
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
        public static TValue Scalar<TValue>(IEnumerable<DatabaseConnection> connections, SixnetDataCommand queryCommand, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var values = new List<TValue>();
            foreach (var conn in connections)
            {
                values.Add(conn.DatabaseProvider.Scalar<TValue>(GetDatabaseSingleCommand<SingleDatabaseCommand>(conn, queryCommand, options)));
            }
            var conversionName = queryCommand?.Queryable?.SelectedFields?.FirstOrDefault()?.FormatSetting?.Name;
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
        public static DataSet QueryMultiple(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            var dataSets = new List<DataSet>();
            foreach (var conn in connections)
            {
                var databaseCommand = GetDatabaseMultipleCommand(conn, queryCommands, options);
                dataSets.Add(conn.DatabaseProvider.QueryMultiple(databaseCommand));
            }
            var finallyDataSet = new DataSet();
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
        public static Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
        public static Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
            IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> queryCommands, DataOperationOptions options = null)
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
        public static List<SixnetDataTable> GetTables(DatabaseConnection connection, DataOperationOptions options = null)
        {
            return connection.DatabaseProvider.GetTables(new DatabaseCommand()
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
        public static int Execute(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> commands, DataOperationOptions options = null)
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
        public static Dictionary<string, TIdentity> InsertAndReturnAutoIdentity<TIdentity>(IEnumerable<DatabaseConnection> connections, IEnumerable<SixnetDataCommand> commands, DataOperationOptions options = null)
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
        public static void BulkInsert(IEnumerable<DatabaseConnection> connections, DataTable dataTable, ISixnetBulkInsertionOptions options = null)
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
        public static void Migrate(IEnumerable<DatabaseConnection> connections, MigrationInfo migrationInfo, DataOperationOptions options = null)
        {
            ValidateConnections(connections);
            foreach (var connection in connections)
            {
                connection.DatabaseProvider.Migrate(new MigrationDatabaseCommand()
                {
                    CancellationToken = options?.CancellationToken,
                    Connection = connection,
                    MigrationInfo = migrationInfo
                });
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
        static TDatabaseCommand GetDatabaseSingleCommand<TDatabaseCommand>(DatabaseConnection connection, SixnetDataCommand dataCommand, DataOperationOptions options) where TDatabaseCommand : SingleDatabaseCommand, new()
        {
            return new TDatabaseCommand()
            {
                Connection = connection,
                CancellationToken = options?.CancellationToken ?? default,
                DataCommand = dataCommand
            };
        }

        /// <summary>
        /// Get database multiple command
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="commands">Commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        static MultipleDatabaseCommand GetDatabaseMultipleCommand(DatabaseConnection connection, IEnumerable<SixnetDataCommand> commands, DataOperationOptions options)
        {
            SixnetDirectThrower.ThrowArgNullIf(commands.IsNullOrEmpty(), $"{nameof(commands)} is null or empty");

            return new MultipleDatabaseCommand()
            {
                Connection = connection,
                CancellationToken = options?.CancellationToken ?? default,
                DataCommands = commands?.ToList() ?? new List<SixnetDataCommand>()
            };
        }

        /// <summary>
        /// Get a database buld insert command
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        static BulkInsertDatabaseCommand GetDatabaseBulkInsertCommand(DatabaseConnection connection, DataTable dataTable, ISixnetBulkInsertionOptions options = null)
        {
            return new BulkInsertDatabaseCommand()
            {
                Connection = connection,
                BulkInsertionOptions = options,
                DataTable = dataTable
            };
        }

        /// <summary>
        /// Validate connections
        /// </summary>
        /// <param name="connections">Database connections</param>
        /// <exception cref="ArgumentNullException"></exception>
        static void ValidateConnections(IEnumerable<DatabaseConnection> connections)
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
