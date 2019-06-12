using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using EZNEW.Develop.Entity;
using EZNEW.Framework.IoC;

namespace EZNEW.Develop.Command
{
    /// <summary>
    ///  Command Execute Manager
    /// </summary>
    public static class CommandExecuteManager
    {
        /// <summary>
        /// get command execute command engines
        /// </summary>
        public static Func<ICommand, Task<List<ICommandEngine>>> GetCommandEnginesAsync { get; set; }

        #region execute

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="commands">commands</param>
        /// <returns>return the execute data numbers</returns>
        internal static int Execute(IEnumerable<ICommand> commands)
        {
            return ExecuteAsync(commands).Result;
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="commands">commands</param>
        /// <returns>return the execute data numbers</returns>
        internal static async Task<int> ExecuteAsync(IEnumerable<ICommand> commands)
        {
            if (commands == null || !commands.Any())
            {
                return 0;
            }
            var cmdGroupEngines = await GroupCommandByEnginesAsync(commands).ConfigureAwait(false);
            if (cmdGroupEngines == null || cmdGroupEngines.Count <= 0)
            {
                return 0;
            }
            int result = 0;
            foreach (var engineGroup in cmdGroupEngines)
            {
                result += await engineGroup.Value.Item1.ExecuteAsync(engineGroup.Value.Item2.ToArray()).ConfigureAwait(false);
            }
            return result;
        }

        #endregion

        #region query

        /// <summary>
        /// execute query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>queried datas</returns>
        internal static IEnumerable<T> Query<T>(ICommand cmd)
        {
            return QueryAsync<T>(cmd).Result;
        }

        /// <summary>
        /// execute query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>queried datas</returns>
        internal static async Task<IEnumerable<T>> QueryAsync<T>(ICommand cmd)
        {
            var groupEngines = await GroupCommandByEnginesAsync(new List<ICommand>() { cmd }).ConfigureAwait(false);
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return new List<T>(0);
            }

            #region single engine

            if (groupEngines.Count == 1)
            {
                return await groupEngines.First().Value.Item1.QueryAsync<T>(cmd).ConfigureAwait(false);
            }

            #endregion

            #region multiple engines

            IEnumerable<T> datas = new List<T>();
            bool notOrder = cmd.Query == null || cmd.Query.Orders.IsNullOrEmpty();
            int dataSize = cmd.Query?.QuerySize ?? 0;
            foreach (var engineGroup in groupEngines)
            {
                var engineDatas = await engineGroup.Value.Item1.QueryAsync<T>(cmd).ConfigureAwait(false);
                datas = datas.Union(engineDatas);
                if (dataSize > 0 && datas.Count() >= dataSize && notOrder)
                {
                    return datas.Take(dataSize);
                }
            }
            if (!notOrder)
            {
                datas = cmd.Query.Order(datas);
            }
            if (dataSize > 0 && datas.Count() > dataSize)
            {
                datas = datas.Take(dataSize);
            }
            return datas;

            #endregion
        }

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>queried datas</returns>
        internal static IPaging<T> QueryPaging<T>(ICommand cmd) where T : BaseEntity<T>
        {
            return QueryPagingAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>queried datas</returns>
        internal static async Task<IPaging<T>> QueryPagingAsync<T>(ICommand cmd) where T : BaseEntity<T>
        {
            var groupEngines = await GroupCommandByEnginesAsync(new List<ICommand>() { cmd }).ConfigureAwait(false);
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return Paging<T>.EmptyPaging();
            }

            #region single engine

            if (groupEngines.Count == 1)
            {
                return await groupEngines.First().Value.Item1.QueryPagingAsync<T>(cmd).ConfigureAwait(false);
            }

            #endregion

            #region multiple engines

            IEnumerable<T> datas = new List<T>();
            int pageSize = cmd.Query.PagingInfo.PageSize;
            int page = cmd.Query.PagingInfo.Page;
            long totalCount = 0;
            cmd.Query.PagingInfo.PageSize = page * pageSize;
            cmd.Query.PagingInfo.Page = 1;
            foreach (var groupEngine in groupEngines)
            {
                var enginePaging = await groupEngine.Value.Item1.QueryPagingAsync<T>(cmd).ConfigureAwait(false);
                datas = datas.Union(enginePaging);
                totalCount += enginePaging.TotalCount;
            }
            if (cmd.Query != null)
            {
                datas = cmd.Query.Order(datas);
            }
            if (datas.Count() > pageSize)
            {
                datas = datas.Skip((page - 1) * pageSize).Take(pageSize);
            }
            return new Paging<T>(page, pageSize, totalCount, datas);

            #endregion
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>data is exist</returns>
        internal static bool Query(ICommand cmd)
        {
            return QueryAsync(cmd).Result;
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>data is exist</returns>
        internal static async Task<bool> QueryAsync(ICommand cmd)
        {
            var groupEngines = await GroupCommandByEnginesAsync(new List<ICommand>() { cmd }).ConfigureAwait(false);
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return false;
            }
            foreach (var groupEngine in groupEngines)
            {
                var result = await groupEngine.Value.Item1.QueryAsync(cmd).ConfigureAwait(false);
                if (result)
                {
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// query single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>query data</returns>
        internal static T QuerySingle<T>(ICommand cmd)
        {
            return QuerySingleAsync<T>(cmd).Result;
        }

        /// <summary>
        /// query single data
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>query data</returns>
        internal static async Task<T> QuerySingleAsync<T>(ICommand cmd)
        {
            T data;
            switch (cmd.Operate)
            {
                case OperateType.Query:
                    data = await QuerySingleObjectAsync<T>(cmd).ConfigureAwait(false);
                    break;
                case OperateType.Max:
                case OperateType.Min:
                case OperateType.Sum:
                case OperateType.Avg:
                case OperateType.Count:
                    data = await QueryAggregateFunctionResultAsync<T>(cmd).ConfigureAwait(false);
                    break;
                default:
                    data = default(T);
                    break;

            }
            return data;
        }

        /// <summary>
        /// query model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">command</param>
        /// <returns></returns>
        static async Task<T> QuerySingleObjectAsync<T>(ICommand cmd)
        {
            var groupEngines = await GroupCommandByEnginesAsync(new List<ICommand>() { cmd });
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return default(T);
            }
            foreach (var groupEngine in groupEngines)
            {
                var data = await groupEngine.Value.Item1.QuerySingleAsync<T>(cmd).ConfigureAwait(false);
                if (data != null)
                {
                    return data;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Aggregate Function
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="cmd">command</param>
        /// <returns>query data</returns>
        static async Task<T> QueryAggregateFunctionResultAsync<T>(ICommand cmd)
        {
            var groupEngines = await GroupCommandByEnginesAsync(new List<ICommand>() { cmd }).ConfigureAwait(false);
            if (groupEngines == null || groupEngines.Count <= 0)
            {
                return default(T);
            }
            List<T> datas = new List<T>(groupEngines.Count);
            foreach (var groupEngine in groupEngines)
            {
                datas.Add(await groupEngine.Value.Item1.QuerySingleAsync<T>(cmd).ConfigureAwait(false));
            }
            if (datas.Count == 1)
            {
                return datas[0];
            }
            dynamic result = default(T);
            switch (cmd.Operate)
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

        #endregion

        #region get command engine

        /// <summary>
        /// get command engine
        /// </summary>
        /// <param name="commands">command</param>
        /// <returns></returns>
        static async Task<Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>> GroupCommandByEnginesAsync(IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>(0);
            }
            if (GetCommandEnginesAsync == null)
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
                throw new EZNEWException("GetCommandEnginesMethodAsync didn't set any value");
            }
            var cmdEngineDict = new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>();
            foreach (var command in commands)
            {
                if (command == null)
                {
                    continue;
                }
                var cmdEngines = await GetCommandEnginesAsync(command).ConfigureAwait(false);
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

        #endregion

        #region calculate sum

        /// <summary>
        /// calculate sum
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">data list</param>
        /// <returns></returns>
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

        #region calculate average

        /// <summary>
        /// calculate average
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">data list</param>
        /// <returns></returns>
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
