using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using EZNEW.Paging;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.Develop.Command;
using System;
using System.Security;
using System.Runtime.CompilerServices;

namespace EZNEW.Data
{
    /// <summary>
    /// Database command executor
    /// </summary>
    public class DatabaseCommandExecutor : ICommandExecutor
    {
        const string identityKey = "eznew_data_defaultdatabasecommandexecutor";

        /// <summary>
        /// Gets the command executor identity key
        /// </summary>
        public string IdentityKey
        {
            get
            {
                return identityKey;
            }
        }

        #region Execute

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        public int Execute(CommandExecuteOptions executeOption, IEnumerable<ICommand> commands)
        {
            return ExecuteAsync(executeOption, commands).Result;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        public int Execute(CommandExecuteOptions executeOption, params ICommand[] commands)
        {
            return ExecuteAsync(executeOption, commands).Result;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        public async Task<int> ExecuteAsync(CommandExecuteOptions executeOption, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            Dictionary<string, List<ICommand>> commandGroup = new Dictionary<string, List<ICommand>>();
            Dictionary<string, DatabaseServer> serverInfos = new Dictionary<string, DatabaseServer>();

            #region Get database servers

            foreach (var cmd in commands)
            {
                var servers = GetServers(cmd);
                foreach (var server in servers)
                {
                    string serverKey = server.Key;
                    if (serverInfos.ContainsKey(serverKey))
                    {
                        commandGroup[serverKey].Add(cmd);
                    }
                    else
                    {
                        commandGroup.Add(serverKey, new List<ICommand>() { cmd });
                        serverInfos.Add(serverKey, server);
                    }
                }
            }

            #endregion

            #region Verify database server provider

            IEnumerable<DatabaseServerType> serverTypeList = serverInfos.Values.Select(c => c.ServerType).Distinct();
            VerifyServerProvider(serverTypeList);

            #endregion

            #region Execute commands

            //Single database server
            if (commandGroup.Count == 1)
            {
                var firstGroup = commandGroup.First();
                var databaseServer = serverInfos[firstGroup.Key];
                var provider = DataManager.GetDatabaseProvider(databaseServer.ServerType);
                return await provider.ExecuteAsync(databaseServer, executeOption, firstGroup.Value).ConfigureAwait(false);
            }

            //Multiple database server
            Task<int>[] executeTasks = new Task<int>[commandGroup.Count];
            int groupIndex = 0;
            foreach (var cmdGroup in commandGroup)
            {
                var databaseServer = serverInfos[cmdGroup.Key];
                var provider = DataManager.GetDatabaseProvider(databaseServer.ServerType);
                executeTasks[groupIndex] = provider.ExecuteAsync(databaseServer, executeOption, cmdGroup.Value.Select(c => c.Clone()));
                groupIndex++;
            }
            return (await Task.WhenAll(executeTasks).ConfigureAwait(false)).Sum();

            #endregion
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return effect data numbers</returns>
        public async Task<int> ExecuteAsync(CommandExecuteOptions executeOption, params ICommand[] commands)
        {
            IEnumerable<ICommand> cmdCollection = commands;
            return await ExecuteAsync(executeOption, cmdCollection).ConfigureAwait(false);
        }

        #endregion

        #region Query datas

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether data is exist</returns>
        public bool Query(ICommand command)
        {
            return QueryAsync(command).Result;
        }

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether data is exist</returns>
        public async Task<bool> QueryAsync(ICommand command)
        {
            var servers = GetServers(command);
            VerifyServerProvider(servers.Select(c => c.ServerType));
            bool result = false;
            foreach (var server in servers)
            {
                var provider = DataManager.GetDatabaseProvider(server.ServerType);
                result = result || await provider.QueryAsync(server, command).ConfigureAwait(false);
                if (result)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        public IEnumerable<T> Query<T>(ICommand command)
        {
            return QueryAsync<T>(command).Result;
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(ICommand command)
        {
            var servers = GetServers(command);
            VerifyServerProvider(servers.Select(c => c.ServerType));
            IEnumerable<T> datas = null;
            if (servers.Count == 1)
            {
                var nowServer = servers[0];
                var provider = DataManager.GetDatabaseProvider(nowServer.ServerType);
                datas = provider.Query<T>(nowServer, command);
            }
            else
            {
                bool notOrder = command.Query == null || command.Query.Orders.IsNullOrEmpty();
                int dataSize = command.Query?.QuerySize ?? 0;
                var entityCompare = new EntityCompare<T>();
                Task<IEnumerable<T>>[] queryTasks = new Task<IEnumerable<T>>[servers.Count];
                int serverIndex = 0;
                foreach (var server in servers)
                {
                    var provider = DataManager.GetDatabaseProvider(server.ServerType);
                    queryTasks[serverIndex] = provider.QueryAsync<T>(server, command);
                    serverIndex++;
                }
                datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false)).SelectMany(c => c).Distinct(entityCompare);
                if (!notOrder)
                {
                    datas = command.Query.Sort(datas);
                }
                if (dataSize > 0 && datas.Count() > dataSize)
                {
                    datas = datas.Take(dataSize);
                }
            }
            return datas;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        public IPaging<T> QueryPaging<T>(ICommand command) where T : BaseEntity<T>, new()
        {
            return QueryPagingAsync<T>(command).Result;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        public async Task<IPaging<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new()
        {
            var servers = GetServers(command);
            VerifyServerProvider(servers.Select(c => c.ServerType));

            #region Single server

            if (servers.Count == 1)
            {
                return await SingleServerPagingAsync<T>(servers[0], command).ConfigureAwait(false);
            }

            #endregion

            #region Multiple server

            int pageSize = command.Query.PagingInfo.PageSize;
            int page = command.Query.PagingInfo.Page;
            command.Query.PagingInfo.PageSize = page * pageSize;
            command.Query.PagingInfo.Page = 1;

            Task<IPaging<T>>[] pagingTasks = new Task<IPaging<T>>[servers.Count];
            int serverIndex = 0;
            foreach (var server in servers)
            {
                pagingTasks[serverIndex] = SingleServerPagingAsync<T>(server, command);
                serverIndex++;
            }
            var allPagings = await Task.WhenAll(pagingTasks).ConfigureAwait(false);

            IEnumerable<T> datas = Array.Empty<T>();
            long totalCount = 0;
            foreach (var paging in allPagings)
            {
                totalCount += paging.TotalCount;
                datas = datas.Union(paging);
            }
            if (command.Query != null)
            {
                datas = command.Query.Sort(datas);
            }
            if (datas.Count() > pageSize)
            {
                datas = datas.Skip((page - 1) * pageSize).Take(pageSize);
            }
            command.Query.PagingInfo.PageSize = pageSize;
            command.Query.PagingInfo.Page = page;
            return Pager.Create(page, pageSize, totalCount, datas);

            #endregion
        }

        /// <summary>
        /// Query paging with single server
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        async Task<IPaging<T>> SingleServerPagingAsync<T>(DatabaseServer server, ICommand command) where T : BaseEntity<T>, new()
        {
            var provider = DataManager.GetDatabaseProvider(server.ServerType);
            IEnumerable<T> datas = await provider.QueryPagingAsync<T>(server, command).ConfigureAwait(false);
            if (datas.IsNullOrEmpty())
            {
                return Pager.Empty<T>();
            }
            return Pager.Create(command.Query.PagingInfo.Page, command.Query.PagingInfo.PageSize, datas.ElementAt(0).GetTotalCount(), datas);
        }

        /// <summary>
        /// Query a single data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public T AggregateValue<T>(ICommand command)
        {
            return AggregateValueAsync<T>(command).Result;
        }

        /// <summary>
        /// Query a single data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public async Task<T> AggregateValueAsync<T>(ICommand command)
        {
            var servers = GetServers(command);
            VerifyServerProvider(servers.Select(c => c.ServerType));

            //Single server
            if (servers.Count == 1)
            {
                var server = servers.First();
                var provider = DataManager.GetDatabaseProvider(server.ServerType);
                return await provider.AggregateValueAsync<T>(server, command).ConfigureAwait(false);
            }

            //Multiple server
            Task<T>[] aggregateTasks = new Task<T>[servers.Count];
            var serverIndex = 0;
            foreach (var server in servers)
            {
                var provider = DataManager.GetDatabaseProvider(server.ServerType);
                aggregateTasks[serverIndex] = provider.AggregateValueAsync<T>(server, command);
                serverIndex++;
            }
            var datas = await Task.WhenAll(aggregateTasks).ConfigureAwait(false);
            dynamic result = default(T);
            switch (command.OperateType)
            {
                case OperateType.Max:
                    result = datas.Max();
                    break;
                case OperateType.Min:
                    result = datas.Min();
                    break;
                case OperateType.Sum:
                case OperateType.Count:
                    result = Sum(datas);
                    break;
                case OperateType.Avg:
                    result = Average(datas);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="command">query command</param>
        /// <returns>Return data</returns>
        public async Task<DataSet> QueryMultipleAsync(ICommand command)
        {
            var servers = GetServers(command);
            VerifyServerProvider(servers.Select(c => c.ServerType));

            //Single server
            if (servers.Count == 1)
            {
                var server = servers.FirstOrDefault();
                var provider = DataManager.GetDatabaseProvider(server.ServerType);
                return await provider.QueryMultipleAsync(server, command).ConfigureAwait(false);
            }

            //Multiple server
            var queryTasks = new Task<DataSet>[servers.Count];
            var serverIndex = 0;
            foreach (var server in servers)
            {
                var provider = DataManager.GetDatabaseProvider(server.ServerType);
                queryTasks[serverIndex] = provider.QueryMultipleAsync(server, command);
                serverIndex++;
            }
            DataSet allDataSet = new DataSet();
            var dataSetArray = await Task.WhenAll(queryTasks).ConfigureAwait(false);
            foreach (var valueDataSet in dataSetArray)
            {
                if (valueDataSet?.Tables.Count < 1)
                {
                    continue;
                }
                foreach (DataTable table in valueDataSet.Tables)
                {
                    allDataSet.Tables.Add(table);
                }
            }
            return allDataSet;
        }

        #endregion

        #region Bulk insert

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        public async Task BulkInsertAsync(DatabaseServer server, DataTable dataTable, IBulkInsertOptions bulkInsertOptions = null)
        {
            if (server == null)
            {
                return;
            }
            var provider = DataManager.GetDatabaseProvider(server.ServerType);
            await provider.BulkInsertAsync(server, dataTable, bulkInsertOptions).ConfigureAwait(false);
        }

        #endregion

        #region Util

        /// <summary>
        /// Get servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return server list</returns>
        static List<DatabaseServer> GetServers(ICommand command)
        {
            if (command == null)
            {
                return new List<DatabaseServer>(0);
            }
            var servers = DataManager.GetDatabaseServers(command);
            if (servers.IsNullOrEmpty())
            {
                throw new EZNEWException("Database server information not obtained from ICommand");
            }
            return servers;
        }

        /// <summary>
        /// Verify database server provider
        /// </summary>
        /// <param name="serverTypes">Database server types</param>
        void VerifyServerProvider(IEnumerable<DatabaseServerType> serverTypes)
        {
            if (serverTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var serverType in serverTypes)
            {
                var databaseProvider = DataManager.GetDatabaseProvider(serverType);
                if (databaseProvider == null)
                {
                    throw new EZNEWException($"No provider for configuring database type {serverType}");
                }
            }
        }

        /// <summary>
        /// Calculate sum
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return sum value</returns>
        dynamic Sum<T>(IEnumerable<T> datas)
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
        dynamic Average<T>(IEnumerable<T> datas)
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
