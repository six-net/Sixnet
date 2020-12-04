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

namespace EZNEW.Develop.Command
{
    /// <summary>
    ///  Command execute manager
    /// </summary>
    public static class CommandExecuteManager
    {
        /// <summary>
        /// Gets or sets the command execute engines
        /// </summary>
        public static Func<ICommand, List<ICommandEngine>> ResolveCommandEngine { get; set; }

        /// <summary>
        /// Gets or sets whether allow none command engine
        /// </summary>
        public static bool AllowNoneCommandEngine { get; set; } = false;

        #region Execute command

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the execute data effect numbers</returns>
        internal static async Task<int> ExecuteAsync(CommandExecuteOptions executeOption, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return 0;
            }
            var cmdEngineGroupDictionary = GroupCommandByEngines(commands);
            return await ExecuteAsync(executeOption, cmdEngineGroupDictionary.Values).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOption">Execute option</param>
        /// <param name="commandEngineGroup">Command engine group</param>
        /// <returns>Return the execute data effect numbers</returns>
        internal static async Task<int> ExecuteAsync(CommandExecuteOptions executeOption, IEnumerable<Tuple<ICommandEngine, List<ICommand>>> commandEngineGroup)
        {
            if (commandEngineGroup.IsNullOrEmpty())
            {
                return 0;
            }
            var groupCount = commandEngineGroup.Count();

            //Single engine
            if (groupCount == 1)
            {
                var firstGroup = commandEngineGroup.First();
                return await firstGroup.Item1.ExecuteAsync(executeOption, firstGroup.Item2).ConfigureAwait(false);
            }

            //Multiple engine
            Task<int>[] valueTasks = new Task<int>[groupCount];
            var groupIndex = 0;
            foreach (var engineGroupItem in commandEngineGroup)
            {
                valueTasks[groupIndex] = engineGroupItem.Item1.ExecuteAsync(executeOption, engineGroupItem.Item2);
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
            var groupEngines = GroupCommandByEngines(new List<ICommand>(1) { command });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return Array.Empty<T>();
            }

            #region Single engine

            if (groupEngines.Count == 1)
            {
                return await groupEngines.First().Value.Item1.QueryAsync<T>(command).ConfigureAwait(false);
            }

            #endregion

            #region Multiple engines

            Task<IEnumerable<T>>[] queryTasks = new Task<IEnumerable<T>>[groupEngines.Count];
            var groupIndex = 0;
            foreach (var engineGroup in groupEngines)
            {
                queryTasks[groupIndex] = engineGroup.Value.Item1.QueryAsync<T>(command);
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
            var groupEngines = GroupCommandByEngines(new List<ICommand>(1) { command });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return Pager.Empty<T>();
            }

            #region Single engine

            if (groupEngines.Count == 1)
            {
                return await groupEngines.First().Value.Item1.QueryPagingAsync<T>(command).ConfigureAwait(false);
            }

            #endregion

            #region Multiple engines

            int pageSize = command.Query.PagingInfo.PageSize;
            int page = command.Query.PagingInfo.Page;
            command.Query.PagingInfo.PageSize = page * pageSize;
            command.Query.PagingInfo.Page = 1;

            //Paging task
            Task<IPaging<T>>[] pagingTasks = new Task<IPaging<T>>[groupEngines.Count];
            var groupIndex = 0;
            foreach (var groupEngine in groupEngines)
            {
                pagingTasks[groupIndex] = groupEngine.Value.Item1.QueryPagingAsync<T>(command);
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
            var groupEngines = GroupCommandByEngines(new List<ICommand>(1) { command });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return false;
            }
            foreach (var groupEngine in groupEngines)
            {
                var result = await groupEngine.Value.Item1.QueryAsync(command).ConfigureAwait(false);
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
            var groupEngines = GroupCommandByEngines(new List<ICommand>(1) { command });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return default;
            }

            //Single engine
            if (groupEngines.Count == 1)
            {
                var firstGroup = groupEngines.First();
                return await firstGroup.Value.Item1.AggregateValueAsync<T>(command).ConfigureAwait(false);
            }

            //Multiple engine
            var aggregateTasks = new Task<T>[groupEngines.Count];
            var groupIndex = 0;
            foreach (var groupEngine in groupEngines)
            {
                aggregateTasks[groupIndex] = groupEngine.Value.Item1.AggregateValueAsync<T>(command);
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
            var groupEngines = GroupCommandByEngines(new List<ICommand>(1) { command });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return new DataSet();
            }

            //Single engine
            if (groupEngines.Count == 1)
            {
                var firstGroup = groupEngines.First();
                return await firstGroup.Value.Item1.QueryMultipleAsync(command).ConfigureAwait(false);
            }

            //Multiple engine
            var queryTasks = new Task<DataSet>[groupEngines.Count];
            var groupIndex = 0;
            foreach (var groupEngine in groupEngines)
            {
                queryTasks[groupIndex] = groupEngine.Value.Item1.QueryMultipleAsync(command);
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

        #region Group command engine

        /// <summary>
        /// Group command engine
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return command engine group</returns>
        static Dictionary<string, Tuple<ICommandEngine, List<ICommand>>> GroupCommandByEngines(IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>(0);
            }
            if (ResolveCommandEngine == null)
            {
                var defaultCmdEngine = ContainerManager.Resolve<ICommandEngine>();
                if (defaultCmdEngine != null)
                {
                    return new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>()
                    {
                        {
                            defaultCmdEngine.IdentityKey,
                            new Tuple<ICommandEngine, List<ICommand>>(defaultCmdEngine,commands.ToList())
                        }
                    };
                }
                throw new EZNEWException($"{nameof(ResolveCommandEngine)} didn't set any value");
            }
            var cmdEngineDict = new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>();
            foreach (var command in commands)
            {
                if (command == null)
                {
                    continue;
                }
                var cmdEngines = ResolveCommandEngine(command);
                if (cmdEngines.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var engine in cmdEngines)
                {
                    if (engine == null)
                    {
                        continue;
                    }
                    var engineKey = engine.IdentityKey;
                    cmdEngineDict.TryGetValue(engineKey, out Tuple<ICommandEngine, List<ICommand>> engineValues);
                    if (engineValues == null)
                    {
                        engineValues = new Tuple<ICommandEngine, List<ICommand>>(engine, new List<ICommand>());
                    }
                    engineValues.Item2.Add(command);
                    cmdEngineDict[engineKey] = engineValues;
                }
            }
            return cmdEngineDict;
        }

        /// <summary>
        /// Gets command engine
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return command engines</returns>
        internal static List<ICommandEngine> GetCommandEngines(ICommand command)
        {
            if (command == null)
            {
                return new List<ICommandEngine>(0);
            }
            List<ICommandEngine> commandEngines = null;
            if (ResolveCommandEngine == null)
            {
                var defaultCmdEngine = ContainerManager.Resolve<ICommandEngine>();
                if (defaultCmdEngine != null)
                {
                    commandEngines = new List<ICommandEngine>(1) { defaultCmdEngine };
                }
            }
            else
            {
                commandEngines = ResolveCommandEngine(command);
            }
            if (!AllowNoneCommandEngine && commandEngines.IsNullOrEmpty())
            {
                throw new EZNEWException("Didn't set any command engines");
            }
            return commandEngines ?? new List<ICommandEngine>(0);
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
    }
}
