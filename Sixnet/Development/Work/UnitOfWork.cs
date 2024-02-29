using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Domain.Event;
using Sixnet.Development.Domain.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Wrok manager
    /// </summary>
    public static class UnitOfWork
    {
        static UnitOfWork()
        {
            SubscribeCreateWorkEvent(w =>
            {
                SixnetMessager.Init();
            });
            SubscribeWorkSuccessEvent((work) =>
            {
                SixnetMessager.Commit(true);
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
        static readonly AsyncLocal<DefaultWork> CurrentWork = new();

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
        public static ISixnetWork Create(DataIsolationLevel? isolationLevel = null)
        {
            return Create(Array.Empty<SixnetDatabaseServer>(), isolationLevel);
        }

        /// <summary>
        /// Create a new work
        /// </summary>
        /// <param name="databaseServerConfigNames">Database server config names</param>
        /// <param name="isolationLevel">Isllation level</param>
        /// <returns></returns>
        public static ISixnetWork Create(IEnumerable<string> databaseServerConfigNames, DataIsolationLevel? isolationLevel = null)
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
        public static ISixnetWork Create(IEnumerable<SixnetDatabaseServer> servers, DataIsolationLevel? isolationLevel = null)
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
        internal static void PublishDomainEvent(IEnumerable<ISixnetDomainEvent> domainEvents)
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
        internal static void PublishDomainEvent(params ISixnetDomainEvent[] domainEvents)
        {
            IEnumerable<ISixnetDomainEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        #endregion

        #endregion
    }
}
