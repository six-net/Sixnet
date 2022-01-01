using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using EZNEW.Exceptions;
using EZNEW.Development.Entity;
using EZNEW.DependencyInjection;
using EZNEW.Paging;
using EZNEW.Data;

namespace EZNEW.Development.Command
{
    /// <summary>
    ///  Command execution manager
    /// </summary>
    public static class CommandExecutionManager
    {
        #region Fields

        /// <summary>
        /// Gets command executor delegate
        /// </summary>
        private static Func<ICommand, IEnumerable<ICommandExecutor>> GetCommandExecutorDelegate { get; set; } = null;

        /// <summary>
        /// Indicates whether allow none command executor
        /// </summary>
        public static bool AllowNoneCommandExecutor { get; set; } = false;

        #endregion

        #region Configure executor

        /// <summary>
        /// Configure executor
        /// </summary>
        /// <param name="getCommandExecutorDelegate">Get command executor delegate</param>
        public static void ConfigureExecutor(Func<ICommand, IEnumerable<ICommandExecutor>> getCommandExecutorDelegate)
        {
            GetCommandExecutorDelegate = getCommandExecutorDelegate;
        }

        #endregion

        #region Execute command

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return affected data number</returns>
        internal static async Task<int> ExecuteAsync(CommandExecutionOptions executionOptions, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            var cmdExecutorGroupDictionary = GroupCommandByExecutor(commands);
            return await ExecuteAsync(executionOptions, cmdExecutorGroupDictionary.Values).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executionOptions">Execution options</param>
        /// <param name="commandExecutorGroup">Command executor group</param>
        /// <returns>Return affected data number</returns>
        internal static async Task<int> ExecuteAsync(CommandExecutionOptions executionOptions, IEnumerable<Tuple<ICommandExecutor, List<ICommand>>> commandExecutorGroup)
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
                return await firstGroup.Item1.ExecuteAsync(executionOptions, firstGroup.Item2).ConfigureAwait(false);
            }

            //Multiple executor
            Task<int>[] resultTasks = new Task<int>[groupCount];
            var groupIndex = 0;
            foreach (var executorGroupItem in commandExecutorGroup)
            {
                resultTasks[groupIndex] = executorGroupItem.Item1.ExecuteAsync(executionOptions, executorGroupItem.Item2?.Select(c => c.Clone()));
                groupIndex++;
            }
            return (await Task.WhenAll(resultTasks).ConfigureAwait(false)).Sum();
        }

        #endregion

        #region Query

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        internal static async Task<IEnumerable<T>> QueryAsync<T>(ICommand command) where T : BaseEntity<T>, new()
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
            bool notSort = command.Query?.Sorts.IsNullOrEmpty() ?? true;
            int dataSize = command.Query?.QuerySize ?? 0;
            if (!notSort)
            {
                datas = command.Query.SortData(datas);
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
        internal static async Task<PagingInfo<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new()
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
            Task<PagingInfo<T>>[] pagingTasks = new Task<PagingInfo<T>>[groupExecutors.Count];
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
                if (paging == null)
                {
                    continue;
                }
                totalCount += paging.TotalCount;
                if (!paging.Items.IsNullOrEmpty())
                {
                    datas = datas.Union(paging.Items);
                }
            }
            if (command.Query != null)
            {
                datas = command.Query.SortData(datas);
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
        /// Determines whether exists data
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether exists data</returns>
        internal static async Task<bool> ExistsAsync(ICommand command)
        {
            var groupExecutors = GroupCommandByExecutor(new List<ICommand>(1) { command });
            if (groupExecutors == null || groupExecutors.Count <= 0)
            {
                return false;
            }
            foreach (var groupExecutor in groupExecutors)
            {
                var result = await groupExecutor.Value.Item1.ExistsAsync(command).ConfigureAwait(false);
                if (result)
                {
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Aggregation value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return value</returns>
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
            var aggregationTasks = new Task<T>[groupExecutors.Count];
            var groupIndex = 0;
            foreach (var groupExecutor in groupExecutors)
            {
                aggregationTasks[groupIndex] = groupExecutor.Value.Item1.AggregateValueAsync<T>(command);
                groupIndex++;
            }
            var datas = await Task.WhenAll(aggregationTasks).ConfigureAwait(false);
            dynamic result = default(T);
            switch (command.OperationType)
            {
                case CommandOperationType.Max:
                    result = datas.Max();
                    break;
                case CommandOperationType.Min:
                    result = datas.Min();
                    break;
                case CommandOperationType.Sum:
                case CommandOperationType.Count:
                    result = Sum(datas);
                    break;
                case CommandOperationType.Avg:
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
            if (GetCommandExecutorDelegate == null)
            {
                var defaultCmdExecutor = ContainerManager.Resolve<ICommandExecutor>();
                if (defaultCmdExecutor != null)
                {
                    return new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>()
                    {
                        {
                            defaultCmdExecutor.IdentityValue,
                            new Tuple<ICommandExecutor, List<ICommand>>(defaultCmdExecutor,commands.ToList())
                        }
                    };
                }
                throw new EZNEWException($"Not set {nameof(GetCommandExecutorDelegate)}");
            }
            var cmdExecutorDict = new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>();
            foreach (var command in commands)
            {
                if (command == null)
                {
                    continue;
                }
                var cmdExecutors = GetCommandExecutorDelegate(command);
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
                    var executorKey = executor.IdentityValue;
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
            if (GetCommandExecutorDelegate == null)
            {
                var defaultCmdExecutor = ContainerManager.Resolve<ICommandExecutor>();
                if (defaultCmdExecutor != null)
                {
                    commandExecutors = new List<ICommandExecutor>(1) { defaultCmdExecutor };
                }
            }
            else
            {
                commandExecutors = GetCommandExecutorDelegate(command)?.ToList() ?? new List<ICommandExecutor>(0);
            }
            if (!AllowNoneCommandExecutor && commandExecutors.IsNullOrEmpty())
            {
                throw new EZNEWException("Not set command executor");
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
        internal static async Task BulkInsertAsync(DatabaseServer server, DataTable dataTable, IBulkInsertionOptions bulkInsertOptions = null)
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
