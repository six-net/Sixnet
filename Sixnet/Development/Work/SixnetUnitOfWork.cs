// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;
using Sixnet.Component.Retry;
using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Events;
using Sixnet.Development.Message;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Wrok manager
    /// </summary>
    public static class SixnetUnitOfWork
    {
        static SixnetUnitOfWork()
        {
            SubscribeCreateWorkEvent(w =>
            {
                SixnetMessager.Init();
            });
            SubscribeWorkSuccessEvent((work) =>
            {
                _ = SixnetMessager.CommitAsync();
            });
            SubscribeWorkRollbackEvent(w =>
            {
                SixnetMessager.Clear();
            });
            SubscribeWorkFailEvent((work) =>
            {
                SixnetMessager.Clear();
            });
        }

        #region Fields

        /// <summary>
        /// Current work
        /// </summary>
        static readonly AsyncLocal<SixnetDefaultWork> CurrentWork = new();

        /// <summary>
        /// create work event handler
        /// </summary>
        static Action<ISixnetWork> CreateWorkEventHandler;

        /// <summary>
        /// Commit success event handler
        /// </summary>
        static Action<ISixnetWork> WorkSuccessEventHandler;

        /// <summary>
        /// Commit fail event handler
        /// </summary>
        static Action<ISixnetWork> WorkFailEventHandler;

        /// <summary>
        /// Work rollback event handler
        /// </summary>
        static Action<ISixnetWork> WorkRollbackEventHandler;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current work
        /// </summary>
        public static ISixnetWork Current
        {
            get
            {
                return CurrentWork?.Value;
            }
            internal set
            {
                CurrentWork.Value = value as SixnetDefaultWork;
            }
        }

        #endregion

        #region Methods

        #region Create work

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>Return a new work object</returns>
        public static ISixnetWork Create(SixnetDataIsolationLevel? isolationLevel = null)
        {
            return Create(Array.Empty<SixnetDatabaseServer>(), isolationLevel);
        }

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="isolationLevel">Isllation level</param>
        /// <returns></returns>
        public static ISixnetWork Create(IEnumerable<string> databaseServerConfigNames, SixnetDataIsolationLevel? isolationLevel = null)
        {
            if (databaseServerConfigNames.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(databaseServerConfigNames));
            }
            var servers = SixnetDataManager.GetDatabaseServers(databaseServerConfigNames.ToArray());
            return Create(servers, isolationLevel);
        }

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="isolationLevel">Isllation level</param>
        /// <returns></returns>
        public static ISixnetWork Create(IEnumerable<SixnetDatabaseServer> servers, SixnetDataIsolationLevel? isolationLevel = null)
        {
            return Current ?? new SixnetDefaultWork(servers, isolationLevel);
        }

        #endregion

        #region Work event

        #region Create work event

        /// <summary>
        /// Subscribe create work event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeCreateWorkEvent(IEnumerable<Action<ISixnetWork>> eventHandlers)
        {
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    CreateWorkEventHandler += handler;
                }
            }
        }

        /// <summary>
        /// Subscribe create work event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeCreateWorkEvent(params Action<ISixnetWork>[] eventHandlers)
        {
            IEnumerable<Action<ISixnetWork>> handlerCollection = eventHandlers;
            SubscribeCreateWorkEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger create work event
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void TriggerCreateWorkEvent(ISixnetWork work)
        {
            CreateWorkEventHandler?.Invoke(work);
        }

        #endregion

        #region Work commit success event

        /// <summary>
        /// Subscribe work commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkSuccessEvent(IEnumerable<Action<ISixnetWork>> eventHandlers)
        {
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    WorkSuccessEventHandler += handler;
                }
            }
        }

        /// <summary>
        /// Subscribe work commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkSuccessEvent(params Action<ISixnetWork>[] eventHandlers)
        {
            IEnumerable<Action<ISixnetWork>> handlerCollection = eventHandlers;
            SubscribeWorkSuccessEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkSuccessEvent(ISixnetWork work)
        {
            ThreadPool.QueueUserWorkItem(s => { WorkSuccessEventHandler(work); });
        }

        #endregion

        #region Work commit fail event

        /// <summary>
        /// Subscribe work commit fail event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkFailEvent(IEnumerable<Action<ISixnetWork>> eventHandlers)
        {
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    WorkFailEventHandler += handler;
                }
            }
        }

        /// <summary>
        /// Subscribe work commit fail event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkFailEvent(params Action<ISixnetWork>[] eventHandlers)
        {
            IEnumerable<Action<ISixnetWork>> handlerCollection = eventHandlers;
            SubscribeWorkFailEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit fail event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkFailEvent(ISixnetWork work)
        {
            ThreadPool.QueueUserWorkItem(s => { WorkFailEventHandler(work); });
        }

        #endregion

        #region Work rollback event

        /// <summary>
        /// Subscribe work rollback event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkRollbackEvent(IEnumerable<Action<ISixnetWork>> eventHandlers)
        {
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    WorkRollbackEventHandler += handler;
                }
            }
        }

        /// <summary>
        /// Subscribe work rollback event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkRollbackEvent(params Action<ISixnetWork>[] eventHandlers)
        {
            IEnumerable<Action<ISixnetWork>> handlerCollection = eventHandlers;
            SubscribeWorkRollbackEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger create work event
        /// </summary>
        /// <param name="work">Work object</param>
        internal static void TriggerWorkRollbackEvent(ISixnetWork work)
        {
            ThreadPool.QueueUserWorkItem(s => { WorkRollbackEventHandler(work); });
        }

        #endregion

        #endregion

        #region Domain event

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal static void PublishDomainEvent(IEnumerable<ISixnetEvent> domainEvents)
        {
            if (!domainEvents.IsNullOrEmpty() && CurrentWork.Value != null)
            {
                CurrentWork.Value.PublishDomainEvent(domainEvents);
            }
        }

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal static void PublishDomainEvent(params ISixnetEvent[] domainEvents)
        {
            IEnumerable<ISixnetEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        #endregion

        #region Execute

        static SixnetRetryPipeline GetRetryPipeline(ISixnetWork work, SixnetUnitOfWorkOptions options, SixnetUnitOfWorkSetting setting)
        {
            var retryTimes = 0;
            if (!options.NotRetry && !setting.NotRetry)
            {
                retryTimes = setting.RetryTimes.HasValue && setting.RetryTimes.Value > 0
                    ? setting.RetryTimes.Value
                    : options.RetryTimes;
                if (retryTimes < 1)
                {
                    retryTimes = 1;
                }
            }
            return new SixnetRetryPipeline()
            {
                Times = retryTimes,
                ToRetry = options.AllowRetry,
                OnBeforeRetry = ctx =>
                {
                    work.Rollback();
                }
            };
        }

        static SixnetRetryPipeline<T> GetRetryPipeline<T>(ISixnetWork work, SixnetUnitOfWorkOptions options, SixnetUnitOfWorkSetting setting)
        {
            var retryTimes = 0;
            if (!options.NotRetry && !setting.NotRetry)
            {
                retryTimes = setting.RetryTimes.HasValue && setting.RetryTimes.Value > 0
                    ? setting.RetryTimes.Value
                    : options.RetryTimes;
                if (retryTimes < 1)
                {
                    retryTimes = 1;
                }
            }
            return new SixnetRetryPipeline<T>()
            {
                Times = retryTimes,
                When = options.AllowRetry,
                OnBeforeRetry = ctx =>
                {
                    work.Rollback();
                }
            };
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="action">Func</param>
        /// <param name="configure">Configure</param>
        public static void Execute(Action<SixnetUnitOfWorkExecutionContext> action, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(setting.IsolationLevel))
            {
                ExecuteCore(work, action, setting);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="action">Action</param>
        /// <param name="configure">Configure</param>
        public static void Execute(IEnumerable<string> databaseServerConfigNames, Action<SixnetUnitOfWorkExecutionContext> action, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(databaseServerConfigNames, setting.IsolationLevel))
            {
                ExecuteCore(work, action, setting);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="action">Action</param>
        /// <param name="configure">Configure</param>
        public static void Execute(IEnumerable<SixnetDatabaseServer> servers, Action<SixnetUnitOfWorkExecutionContext> action, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(servers, setting.IsolationLevel))
            {
                ExecuteCore(work, action, setting);
            }
        }

        static void ExecuteCore(ISixnetWork work, Action<SixnetUnitOfWorkExecutionContext> action, SixnetUnitOfWorkSetting setting)
        {
            var workOptions = SixnetContainer.GetOptions<SixnetUnitOfWorkOptions>();
            var retryPipeline = GetRetryPipeline(work, workOptions, setting);
            retryPipeline.Action = () =>
            {
                action?.Invoke(new SixnetUnitOfWorkExecutionContext()
                {
                    Work = work
                });
            };
            // execute
            retryPipeline.Execute();
            // commit
            if (setting.AutoCommit)
            {
                work.Commit();
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task ExecuteAsync(Func<SixnetUnitOfWorkExecutionContext, Task> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(setting.IsolationLevel))
            {
                await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task ExecuteAsync(IEnumerable<string> databaseServerConfigNames, Func<SixnetUnitOfWorkExecutionContext, Task> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(databaseServerConfigNames, setting.IsolationLevel))
            {
                await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="servers">Servers</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task ExecuteAsync(IEnumerable<SixnetDatabaseServer> servers, Func<SixnetUnitOfWorkExecutionContext, Task> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(servers, setting.IsolationLevel))
            {
                await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        static async Task ExecuteCoreAsync(ISixnetWork work, Func<SixnetUnitOfWorkExecutionContext, Task> func, SixnetUnitOfWorkSetting setting)
        {
            var workOptions = SixnetContainer.GetOptions<SixnetUnitOfWorkOptions>();
            var retryPipeline = GetRetryPipeline(work, workOptions, setting);
            retryPipeline.ActionAsync = async () =>
            {
                if (func != null)
                {
                    await func(new SixnetUnitOfWorkExecutionContext()
                    {
                        Work = work
                    }).ConfigureAwait(false);
                }
            };
            // execute
            await retryPipeline.ExecuteAsync().ConfigureAwait(false);
            // commit
            if (setting.AutoCommit)
            {
                await work.CommitAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static T Execute<T>(Func<SixnetUnitOfWorkExecutionContext, T> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(setting.IsolationLevel))
            {
                return ExecuteCore(work, func, setting);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static T Execute<T>(IEnumerable<string> databaseServerConfigNames, Func<SixnetUnitOfWorkExecutionContext, T> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(databaseServerConfigNames, setting.IsolationLevel))
            {
                return ExecuteCore(work, func, setting);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="servers">Servers</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static T Execute<T>(IEnumerable<SixnetDatabaseServer> servers, Func<SixnetUnitOfWorkExecutionContext, T> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(servers, setting.IsolationLevel))
            {
                return ExecuteCore(work, func, setting);
            }
        }

        static T ExecuteCore<T>(ISixnetWork work, Func<SixnetUnitOfWorkExecutionContext, T> func, SixnetUnitOfWorkSetting setting)
        {
            var workOptions = SixnetContainer.GetOptions<SixnetUnitOfWorkOptions>();
            var retryPipeline = GetRetryPipeline<T>(work, workOptions, setting);
            retryPipeline.Func = () =>
            {
                if (func != null)
                {
                    return func(new SixnetUnitOfWorkExecutionContext()
                    {
                        Work = work
                    });
                }
                return default;
            };
            // execute
            var res = retryPipeline.Execute();
            // commit
            if (setting.AutoCommit)
            {
                work.Commit();
            }
            return res;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task<T> ExecuteAsync<T>(Func<SixnetUnitOfWorkExecutionContext, Task<T>> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(setting.IsolationLevel))
            {
                return await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task<T> ExecuteAsync<T>(IEnumerable<string> databaseServerConfigNames, Func<SixnetUnitOfWorkExecutionContext, Task<T>> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(databaseServerConfigNames, setting.IsolationLevel))
            {
                return await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="servers">Servers</param>
        /// <param name="func">Func</param>
        /// <param name="configure">Configure</param>
        public static async Task<T> ExecuteAsync<T>(IEnumerable<SixnetDatabaseServer> servers, Func<SixnetUnitOfWorkExecutionContext, Task<T>> func, Action<SixnetUnitOfWorkSetting> configure = null)
        {
            var setting = SixnetUnitOfWorkSetting.GetDefaultSetting();
            configure?.Invoke(setting);
            using (var work = Create(servers, setting.IsolationLevel))
            {
                return await ExecuteCoreAsync(work, func, setting).ConfigureAwait(false);
            }
        }

        static async Task<T> ExecuteCoreAsync<T>(ISixnetWork work, Func<SixnetUnitOfWorkExecutionContext, Task<T>> func, SixnetUnitOfWorkSetting setting)
        {
            var workOptions = SixnetContainer.GetOptions<SixnetUnitOfWorkOptions>();
            var retryPipeline = GetRetryPipeline<T>(work, workOptions, setting);
            retryPipeline.FuncAsync = async () =>
            {
                if (func != null)
                {
                    return await func(new SixnetUnitOfWorkExecutionContext()
                    {
                        Work = work
                    }).ConfigureAwait(false);
                }
                return default;
            };
            // execute
            var res = await retryPipeline.ExecuteAsync().ConfigureAwait(false);
            // commit
            if (setting.AutoCommit)
            {
                await work.CommitAsync().ConfigureAwait(false);
            }
            return res;
        }

        #endregion

        #endregion
    }
}
