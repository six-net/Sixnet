using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Command;
using EZNEW.Paging;
using System.Runtime.CompilerServices;
using EZNEW.Develop.Message;
using EZNEW.Data;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Wrok manager
    /// </summary>
    public class WorkManager
    {
        static WorkManager()
        {
            SubscribeCreateWorkEvent(w =>
            {
                MessageManager.Init();
            });
            SubscribeWorkCommitSuccessEvent((work, res, cmds) =>
            {
                MessageManager.Commit(true);
            });
            SubscribeWorkRollbackEvent(w =>
            {
                MessageManager.Clear();
            });
            SubscribeWorkCommitFailEvent((work, res, cmds) =>
            {
                MessageManager.Clear();
            });
        }

        #region Fields

        /// <summary>
        /// Current work
        /// </summary>
        static AsyncLocal<IWork> current = new AsyncLocal<IWork>();

        /// <summary>
        /// create work event handler
        /// </summary>
        static Action<IWork> CreateWorkEventHandler;

        /// <summary>
        /// Commit success event handler
        /// </summary>
        static readonly List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> WorkCommitSuccessEventHandlers = new List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>>();

        /// <summary>
        /// Commit fail event handler
        /// </summary>
        static readonly List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> WorkCommitFailEventHandlers = new List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>>();

        /// <summary>
        /// Work rollback event handler
        /// </summary>
        static readonly List<Action<IWork>> WorkRollbackEventHandlers = new List<Action<IWork>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current IUnitOfWork Object
        /// </summary>
        public static IWork Current
        {
            get
            {
                return current?.Value;
            }
            internal set
            {
                current.Value = value;
            }
        }

        #endregion

        #region Methods

        #region Work event

        #region Create work event

        /// <summary>
        /// Subscribe create work event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeCreateWorkEvent(IEnumerable<Action<IWork>> eventHandlers)
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in eventHandlers)
            {
                CreateWorkEventHandler += handler;
            }
        }

        /// <summary>
        /// Subscribe create work event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeCreateWorkEvent(params Action<IWork>[] eventHandlers)
        {
            IEnumerable<Action<IWork>> handlerCollection = eventHandlers;
            SubscribeCreateWorkEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger create work event
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void TriggerCreateWorkEvent(IWork work)
        {
            CreateWorkEventHandler?.Invoke(work);
        }

        #endregion

        #region Work commit success event

        /// <summary>
        /// Subscribe work commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkCommitSuccessEvent(IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> eventHandlers)
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            WorkCommitSuccessEventHandlers.AddRange(eventHandlers);
        }

        /// <summary>
        /// Subscribe work commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkCommitSuccessEvent(params Action<IWork, WorkCommitResult, IEnumerable<ICommand>>[] eventHandlers)
        {
            IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> handlerCollection = eventHandlers;
            SubscribeWorkCommitSuccessEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkCommitSuccessEvent(IWork work, WorkCommitResult commitResult, IEnumerable<ICommand> commands)
        {
            if (WorkCommitSuccessEventHandlers == null)
            {
                return;
            }
            foreach (var handler in WorkCommitSuccessEventHandlers)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(s => { eventHandler(work, commitResult, commands); });
            }
        }

        #endregion

        #region Work commit fail event

        /// <summary>
        /// Subscribe work commit fail event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkCommitFailEvent(IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> eventHandlers)
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            WorkCommitFailEventHandlers.AddRange(eventHandlers);
        }

        /// <summary>
        /// Subscribe work commit fail event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkCommitFailEvent(params Action<IWork, WorkCommitResult, IEnumerable<ICommand>>[] eventHandlers)
        {
            IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> handlerCollection = eventHandlers;
            SubscribeWorkCommitFailEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit fail event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkCommitFailEvent(IWork work, WorkCommitResult commitResult, IEnumerable<ICommand> commands)
        {
            if (WorkCommitFailEventHandlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in WorkCommitFailEventHandlers)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(s => { eventHandler(work, commitResult, commands); });
            }
        }

        #endregion

        #region Work rollback event

        /// <summary>
        /// Subscribe work rollback event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkRollbackEvent(IEnumerable<Action<IWork>> eventHandlers)
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            WorkRollbackEventHandlers.AddRange(eventHandlers);
        }

        /// <summary>
        /// Subscribe work rollback event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkRollbackEvent(params Action<IWork>[] eventHandlers)
        {
            IEnumerable<Action<IWork>> handlerCollection = eventHandlers;
            SubscribeWorkRollbackEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger create work event
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void TriggerWorkRollbackEvent(IWork work)
        {
            if (WorkRollbackEventHandlers == null)
            {
                return;
            }
            foreach (var handler in WorkRollbackEventHandlers)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(state => eventHandler(work));
            }
        }

        #endregion

        #endregion

        #region Activation record

        /// <summary>
        /// Register activation record
        /// </summary>
        /// <param name="records">Activation records</param>
        public static void RegisterActivationRecord(params IActivationRecord[] records)
        {
            IEnumerable<IActivationRecord> recordCollection = records;
            RegisterActivationRecord(recordCollection);
        }

        /// <summary>
        /// Register activation record
        /// </summary>
        /// <param name="records">Activation records</param>
        public static void RegisterActivationRecord(IEnumerable<IActivationRecord> records)
        {
            Current?.RegisterActivationRecord(records);
        }

        #endregion

        #region Query data

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        public static IEnumerable<T> Query<T>(ICommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return QueryAsync<T>(command).Result;
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(ICommand command)
        {
            if (command?.IsObsolete ?? true)
            {
                return new List<T>(0);
            }
            return await CommandExecuteManager.QueryAsync<T>(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return datas</returns>
        public static IPaging<T> QueryPaging<T>(ICommand command) where T : BaseEntity<T>, new()
        {
            return QueryPagingAsync<T>(command).Result;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data paging</returns>
        public static async Task<IPaging<T>> QueryPagingAsync<T>(ICommand command) where T : BaseEntity<T>, new()
        {
            if (command?.IsObsolete ?? true)
            {
                return Pager.Empty<T>();
            }
            if (command.Query == null)
            {
                command.Query = QueryManager.CreateByEntity<T>();
            }
            if (command.Query.PagingInfo == null)
            {
                command.Query.SetPaging(new PagingFilter());
            }
            return await CommandExecuteManager.QueryPagingAsync<T>(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Reeturn whether data is exist</returns>
        public static bool Query(ICommand command)
        {
            return QueryAsync(command).Result;
        }

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return whether data is exist</returns>
        public static async Task<bool> QueryAsync(ICommand command)
        {
            if (command?.IsObsolete ?? true)
            {
                return false;
            }
            return await CommandExecuteManager.QueryAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Query single data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public static T QuerySingle<T>(ICommand command)
        {
            return QuerySingleAsync<T>(command).Result;
        }

        /// <summary>
        /// Query single data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public static async Task<T> QuerySingleAsync<T>(ICommand command)
        {
            if (command?.IsObsolete ?? true)
            {
                return default;
            }
            if (command.Query == null)
            {
                command.Query = QueryManager.Create();
            }
            command.Query.QuerySize = 1;
            var datas = await CommandExecuteManager.QueryAsync<T>(command).ConfigureAwait(false);
            if (datas.IsNullOrEmpty())
            {
                return default;
            }
            return datas.FirstOrDefault();
        }

        /// <summary>
        /// Query aggregate data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public static T AggregateValue<T>(ICommand command)
        {
            return AggregateValueAsync<T>(command).Result;
        }

        /// <summary>
        /// Query aggregate data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Return data</returns>
        public static async Task<T> AggregateValueAsync<T>(ICommand command)
        {
            return await CommandExecuteManager.AggregateValueAsync<T>(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="command">Execute command</param>
        /// <returns>Return data</returns>
        public static async Task<DataSet> QueryMultipleAsync(ICommand command)
        {
            return await CommandExecuteManager.QueryMultipleAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandTextType">Command text type</param>
        /// <returns>Return data set</returns>
        public static async Task<DataSet> QueryMultipleAsync(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text)
        {
            var rdbCmd = RdbCommand.CreateNewCommand(OperateType.Query, parameters);
            rdbCmd.CommandType = commandTextType;
            rdbCmd.CommandText = commandText;
            return await QueryMultipleAsync(rdbCmd).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="command">Execute command</param>
        /// <returns>Return data set</returns>
        public static DataSet QueryMultiple(ICommand command)
        {
            return QueryMultipleAsync(command).Result;
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandTextType">Command text type</param>
        /// <returns>Return data set</returns>
        public static DataSet QueryMultiple(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text)
        {
            return QueryMultipleAsync(commandText, parameters, commandTextType).Result;
        }

        #endregion

        #region Execute command

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect numbers</returns>
        public static async Task<int> ExecuteAsync(params ICommand[] commands)
        {
            IEnumerable<ICommand> cmdCollection = commands;
            return await ExecuteAsync(cmdCollection).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect numbers</returns>
        public static async Task<int> ExecuteAsync(IEnumerable<ICommand> commands)
        {
            return await ExecuteAsync(CommandExecuteOptions.Default, commands).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the effect data number</returns>
        public static async Task<int> ExecuteAsync(CommandExecuteOptions executeOptions, params ICommand[] commands)
        {
            IEnumerable<ICommand> cmdCollection = commands;
            return await ExecuteAsync(executeOptions, cmdCollection).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the effect data number</returns>
        public static async Task<int> ExecuteAsync(CommandExecuteOptions executeOptions, IEnumerable<ICommand> commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return await Task.FromResult(0).ConfigureAwait(false);
            }
            return await CommandExecuteManager.ExecuteAsync(executeOptions ?? CommandExecuteOptions.Default, commands).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command text
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandTextType">Command text type</param>
        /// <param name="executeOptions">Execute options</param>
        /// <returns>Return the effect data number</returns>
        public static async Task<int> ExecuteAsync(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text, CommandExecuteOptions executeOptions = null)
        {
            var rdbCmd = RdbCommand.CreateNewCommand(OperateType.Query, parameters);
            rdbCmd.CommandType = commandTextType;
            rdbCmd.CommandText = commandText;
            rdbCmd.ExecuteMode = CommandExecuteMode.CommandText;
            return await ExecuteAsync(executeOptions, rdbCmd).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect numbers</returns>
        public static int Execute(params ICommand[] commands)
        {
            return ExecuteAsync(commands).Result;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect numbers</returns>
        public static int Execute(IEnumerable<ICommand> commands)
        {
            return ExecuteAsync(commands).Result;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute option</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return the data effect number</returns>
        public static int Execute(CommandExecuteOptions executeOptions, params ICommand[] commands)
        {
            return ExecuteAsync(executeOptions, commands).Result;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="executeOptions">Execute options</param>
        /// <param name="commands">Commands</param>
        /// <returns>Return data effects number</returns>
        public static int Execute(CommandExecuteOptions executeOptions, IEnumerable<ICommand> commands)
        {
            return ExecuteAsync(executeOptions, commands).Result;
        }

        /// <summary>
        /// Execute command text
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandTextType">Command text type</param>
        /// <param name="executeOptions">Execute options</param>
        /// <returns>Return data effects number</returns>
        public static int Execute(string commandText, object parameters = null, CommandTextType commandTextType = CommandTextType.Text, CommandExecuteOptions executeOptions = null)
        {
            return ExecuteAsync(commandText, parameters, commandTextType, executeOptions).Result;
        }

        #endregion

        #region Create work

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>Return a new work object</returns>
        public static IWork Create(DataIsolationLevel? isolationLevel = null)
        {
            return new DefaultWork()
            {
                IsolationLevel = isolationLevel
            };
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commit current work
        /// </summary>
        public static void Commit()
        {
            Current?.Commit();
        }

        /// <summary>
        /// Commit current work
        /// </summary>
        public static async Task CommitAsync()
        {
            if (Current != null)
            {
                await Current.CommitAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        public static void Rollback()
        {
            Current?.Rollback();
        }

        #endregion

        #region Bulk insert

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="datas">Datas</param>
        /// <param name="tableName">Table name,will use the configuration entity name or object type name when it is null or empty</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        public static void BulkInsert<T>(DatabaseServer server, IEnumerable<T> datas, string tableName = "", IBulkInsertOptions bulkInsertOptions = null)
        {
            BulkInsertAsync(server, datas, tableName, bulkInsertOptions).Wait();
        }

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="server">Database server</param>
        /// <param name="datas">Datas</param>
        /// <param name="tableName">Table name,will use the configuration entity name or object type name when it is null or empty</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        public static async Task BulkInsertAsync<T>(DatabaseServer server, IEnumerable<T> datas, string tableName = "", IBulkInsertOptions bulkInsertOptions = null)
        {
            var datatable = datas.ToDataTable();
            if (datatable == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                Type dataType = typeof(T);
                tableName = DataManager.GetEntityObjectName(DatabaseServerType.Oracle, typeof(T));
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = dataType.Name;
                }
            }
            datatable.TableName = tableName;
            await BulkInsertAsync(server, datas.ToDataTable(), bulkInsertOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        public static void BulkInsert(DatabaseServer server, DataTable dataTable, IBulkInsertOptions bulkInsertOptions = null)
        {
            BulkInsertAsync(server, dataTable, bulkInsertOptions).Wait();
        }

        /// <summary>
        /// Bulk insert datas
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="bulkInsertOptions">Bulk insert options</param>
        public static async Task BulkInsertAsync(DatabaseServer server, DataTable dataTable, IBulkInsertOptions bulkInsertOptions = null)
        {
            await CommandExecuteManager.BulkInsertAsync(server, dataTable, bulkInsertOptions).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
