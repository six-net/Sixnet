using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Domain.Event;
using Sixnet.Development.Message;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Wrok manager
    /// </summary>
    public class UnitOfWork
    {
        static UnitOfWork()
        {
            SubscribeCreateWorkEvent(w =>
            {
                MessageManager.Init();
            });
            SubscribeWorkSuccessEvent((work) =>
            {
                MessageManager.Commit(true);
            });
            SubscribeWorkRollbackEvent(w =>
            {
                MessageManager.Clear();
            });
            SubscribeWorkFailEvent((work) =>
            {
                MessageManager.Clear();
            });
        }

        #region Fields

        /// <summary>
        /// Current work
        /// </summary>
        static readonly AsyncLocal<DefaultWork> CurrentWork = new();

        /// <summary>
        /// create work event handler
        /// </summary>
        static Action<IWork> CreateWorkEventHandler;

        /// <summary>
        /// Commit success event handler
        /// </summary>
        static Action<IWork> WorkSuccessEventHandler;

        /// <summary>
        /// Commit fail event handler
        /// </summary>
        static Action<IWork> WorkFailEventHandler;

        /// <summary>
        /// Work rollback event handler
        /// </summary>
        static Action<IWork> WorkRollbackEventHandler;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current work
        /// </summary>
        public static IWork Current
        {
            get
            {
                return CurrentWork?.Value;
            }
            internal set
            {
                CurrentWork.Value = value as DefaultWork;
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
        public static IWork Create(DataIsolationLevel? isolationLevel = null)
        {
            return Create(Array.Empty<DatabaseServer>(), isolationLevel);
        }

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="isolationLevel">Isllation level</param>
        /// <returns></returns>
        public static IWork Create(IEnumerable<string> databaseServerConfigNames, DataIsolationLevel? isolationLevel = null)
        {
            if (databaseServerConfigNames.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(databaseServerConfigNames));
            }
            var servers = DataManager.GetConfiguredDatabaseServers(databaseServerConfigNames.ToArray());
            return Create(servers, isolationLevel);
        }

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="isolationLevel">Isllation level</param>
        /// <returns></returns>
        public static IWork Create(IEnumerable<DatabaseServer> servers, DataIsolationLevel? isolationLevel = null)
        {
            return Current ?? new DefaultWork(servers, isolationLevel);
        }

        #endregion

        #region Work event

        #region Create work event

        /// <summary>
        /// Subscribe create work event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeCreateWorkEvent(IEnumerable<Action<IWork>> eventHandlers)
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
        public static void SubscribeWorkSuccessEvent(IEnumerable<Action<IWork>> eventHandlers)
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
        public static void SubscribeWorkSuccessEvent(params Action<IWork>[] eventHandlers)
        {
            IEnumerable<Action<IWork>> handlerCollection = eventHandlers;
            SubscribeWorkSuccessEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkSuccessEvent(IWork work)
        {
            ThreadPool.QueueUserWorkItem(s => { WorkSuccessEventHandler(work); });
        }

        #endregion

        #region Work commit fail event

        /// <summary>
        /// Subscribe work commit fail event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkFailEvent(IEnumerable<Action<IWork>> eventHandlers)
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
        public static void SubscribeWorkFailEvent(params Action<IWork>[] eventHandlers)
        {
            IEnumerable<Action<IWork>> handlerCollection = eventHandlers;
            SubscribeWorkFailEvent(handlerCollection);
        }

        /// <summary>
        /// Trigger commit fail event
        /// </summary>
        /// <param name="work">Work object</param>
        /// <param name="commitResult">Work commit result</param>
        /// <param name="commands">Commands</param>
        internal static void TriggerWorkFailEvent(IWork work)
        {
            ThreadPool.QueueUserWorkItem(s => { WorkFailEventHandler(work); });
        }

        #endregion

        #region Work rollback event

        /// <summary>
        /// Subscribe work rollback event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public static void SubscribeWorkRollbackEvent(IEnumerable<Action<IWork>> eventHandlers)
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
            ThreadPool.QueueUserWorkItem(s => { WorkRollbackEventHandler(work); });
        }

        #endregion

        #endregion

        #region Domain event

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal static void PublishDomainEvent(IEnumerable<IDomainEvent> domainEvents)
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
        internal static void PublishDomainEvent(params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        #endregion

        #endregion
    }
}
