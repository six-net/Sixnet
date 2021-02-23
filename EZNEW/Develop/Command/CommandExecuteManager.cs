using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using EZNEW.Fault;
using EZNEW.Develop.Entity;
using EZNEW.DependencyInjection;
using EZNEW.Paging;
using System.Text.RegularExpressions;
using EZNEW.Data;

namespace EZNEW.Develop.Command
{
    /// <summary>
    ///  Command execute manager
    /// </summary>
    public static class CommandExecuteManager
    {
        /// <summary>
        /// Gets or sets the command executors
        /// </summary>
        public static Func<ICommand, List<ICommandExecutor>> GetCommandExecutorProxy { get; set; }

        /// <summary>
        /// Gets or sets whether allow none command executor
        /// </summary>
        public static bool AllowNoneCommandExecutor { get; set; } = false;

        #region Execute command

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the execute data effect numbers</returns>
        internal static async Task<int> ExecuteAsync(CommandExecuteOptions executeOptions, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            var cmdExecutorGroupDictionary = GroupCommandByExecutor(commands);
            return await ExecuteAsync(executeOptions, cmdExecutorGroupDictionary.Values).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute options</param>
        /// <param name="commandExecutorGroup">Command executor group</param>
        /// <returns>Return the execute data effect numbers</returns>
        internal static async Task<int> ExecuteAsync(CommandExecuteOptions executeOptions, IEnumerable<Tuple<ICommandExecutor, List<ICommand>>> commandExecutorGroup)
        {
            if (commandExecutorGroup.IsNullOrEmpty())
            {
                return 0;
            }
            var groupCount = commandExecutorGroup.Count();

            //Single executor
            if (groupCount == 1)
            {
                var firstGroup = commandExecutorGroup.First();
                return await firstGroup.Item1.ExecuteAsync(executeOptions, firstGroup.Item2).ConfigureAwait(false);
            }

            //Multiple executor
            Task<int>[] valueTasks = new Task<int>[groupCount];
            var groupIndex = 0;
            foreach (var executorGroupItem in commandExecutorGroup)
            {
                valueTasks[groupIndex] = executorGroupItem.Item1.ExecuteAsync(executeOptions, executorGroupItem.Item2);
                groupIndex++;
            }
            return (await Task.WhenAll(valueTasks).ConfigureAwait(false)).Sum();
        }

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        internal static async Task<IEnumerable<T>> QueryAsync<T>(ICommand command)
        {
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return Array.Empty<T>();
            }

            #region Single executor

            if (groupExecutors.Count == 1)
            {
                return await groupExecutors.First().Value.Item1.QueryAsync<T>(command).ConfigureAwait(false);
            }

            #endregion

            #region Multiple executors

            Task<IEnumerable<T>>[] queryTasks = new Task<IEnumerable<T>>[groupExecutors.Count];
            var groupIndex = 0;
            foreach (var executorGroup in groupExecutors)
            {
                queryTasks[groupIndex] = executorGroup.Value.Item1.QueryAsync<T>(command);
                groupIndex++;
            }
            var datas = (await Task.WhenAll(queryTasks).ConfigureAwait(false)).SelectMany(c => c);
            bool notOrder = command.Query?.Orders.IsNullOrEmpty() ?? true;
            int dataSize = command.Query?.QuerySize ?? 0;
            if (!notOrder)
            {
                datas = command.Query.Sort(datas);
            }
            if (dataSize > 0 && datas.Count() > dataSize)
            {
                datas = datas.Take(dataSize);
            }
            return datas;

            #endregion
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        internal static async Task<IPaging<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new()
        {
            if (command.Query?.PagingInfo == null)
            {
                throw new EZNEWException($"Paging information is not set");
            }
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return Pager.Empty<T>();
            }

            #region Single executor

            if (groupExecutors.Count == 1)
            {
                return await groupExecutors.First().Value.Item1.QueryPagingAsync<T>(command).ConfigureAwait(false);
            }

            #endregion

            #region Multiple executors

            int pageSize = command.Query.PagingInfo.PageSize;
            int page = command.Query.PagingInfo.Page;
            command.Query.PagingInfo.PageSize = page * pageSize;
            command.Query.PagingInfo.Page = 1;

            //Paging task
            Task<IPaging<T>>[] pagingTasks = new Task<IPaging<T>>[groupExecutors.Count];
            var groupIndex = 0;
            foreach (var groupExecutor in groupExecutors)
            {
                pagingTasks[groupIndex] = groupExecutor.Value.Item1.QueryPagingAsync<T>(command);
                groupIndex++;
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
        /// Determine whether does the data exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether does the data exist</returns>
        internal static async Task<bool> QueryAsync(ICommand command)
        {
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return false;
            }
            foreach (var groupExecutor in groupExecutors)
            {
                var result = await groupExecutor.Value.Item1.QueryAsync(command).ConfigureAwait(false);
                if (result)
                {
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Aggregate value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        internal static async Task<T> AggregateValueAsync<T>(ICommand command)
        {
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return default;
            }

            //Single executor
            if (groupExecutors.Count == 1)
            {
                var firstGroup = groupExecutors.First();
                return await firstGroup.Value.Item1.AggregateValueAsync<T>(command).ConfigureAwait(false);
            }

            //Multiple executor
            var aggregateTasks = new Task<T>[groupExecutors.Count];
            var groupIndex = 0;
            foreach (var groupExecutor in groupExecutors)
            {
                aggregateTasks[groupIndex] = groupExecutor.Value.Item1.AggregateValueAsync<T>(command);
                groupIndex++;
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
        /// <param name="command">Command</param>
        /// <returns>Return data set</returns>
        internal static async Task<DataSet> QueryMultipleAsync(ICommand command)
        {
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return new DataSet();
            }

            //Single executor
            if (groupExecutors.Count == 1)
            {
                var firstGroup = groupExecutors.First();
                return await firstGroup.Value.Item1.QueryMultipleAsync(command).ConfigureAwait(false);
            }

            //Multiple executor
            var queryTasks = new Task<DataSet>[groupExecutors.Count];
            var groupIndex = 0;
            foreach (var groupExecutor in groupExecutors)
            {
                queryTasks[groupIndex] = groupExecutor.Value.Item1.QueryMultipleAsync(command);
                groupIndex++;
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

        #region Group command by executor

        /// <summary>
        /// Group command executor
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return command executor group</returns>
        static Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>> GroupCommandByExecutor(IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>(0);
            }
            if (GetCommandExecutorProxy == null)
            {
                var defaultCmdExecutor = ContainerManager.Resolve<ICommandExecutor>();
                if (defaultCmdExecutor != null)
                {
                    return new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>()
                    {
                        {
                            defaultCmdExecutor.IdentityKey,
                            new Tuple<ICommandExecutor, List<ICommand>>(defaultCmdExecutor,commands.ToList())
                        }
                    };
                }
                throw new EZNEWException($"{nameof(GetCommandExecutorProxy)} didn't set any value");
            }
            var cmdExecutorDict = new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>();
            foreach (var command in commands)
            {
                if (command == null)
                {
                    continue;
                }
                var cmdExecutors = GetCommandExecutorProxy(command);
                if (cmdExecutors.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var executor in cmdExecutors)
                {
                    if (executor == null)
                    {
                        continue;
                    }
                    var executorKey = executor.IdentityKey;
                    cmdExecutorDict.TryGetValue(executorKey, out Tuple<ICommandExecutor, List<ICommand>> executorValues);
                    if (executorValues == null)
                    {
                        executorValues = new Tuple<ICommandExecutor, List<ICommand>>(executor, new List<ICommand>());
                    }
                    executorValues.Item2.Add(command);
                    cmdExecutorDict[executorKey] = executorValues;
                }
            }
            return cmdExecutorDict;
        }

        /// <summary>
        /// Gets command executors
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return command executors</returns>
        internal static List<ICommandExecutor> GetCommandExecutors(ICommand command)
        {
            if (command == null)
            {
                return new List<ICommandExecutor>(0);
            }
            List<ICommandExecutor> commandExecutors = null;
            if (GetCommandExecutorProxy == null)
            {
                var defaultCmdExecutor = ContainerManager.Resolve<ICommandExecutor>();
                if (defaultCmdExecutor != null)
                {
                    commandExecutors = new List<ICommandExecutor>(1) { defaultCmdExecutor };
                }
            }
            else
            {
                commandExecutors = GetCommandExecutorProxy(command);
            }
            if (!AllowNoneCommandExecutor && commandExecutors.IsNullOrEmpty())
            {
                throw new EZNEWException("Didn't set any command executors");
            }
            return commandExecutors ?? new List<ICommandExecutor>(0);
        }

        #endregion

        #region Calculate sum value

        /// <summary>
        /// Calculate sum value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return the sum value</returns>
        static dynamic Sum<T>(IEnumerable<T> datas)
        {
            dynamic result = default(T);
            foreach (dynamic data in datas)
            {
                result += data;
            }
            return result;
        }

        #endregion

        #region Calculate average value

        /// <summary>
        /// Calculate average value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return the average value</returns>
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

        #region Bulk insert

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        internal static async Task BulkInsertAsync(DatabaseServer server, DataTable dataTable, IBulkInsertOptions bulkInsertOptions = null)
        {
            var defaultCmdExecutor = ContainerManager.Resolve<ICommandExecutor>();
            if (defaultCmdExecutor != null)
            {
                await defaultCmdExecutor.BulkInsertAsync(server, dataTable, bulkInsertOptions).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
