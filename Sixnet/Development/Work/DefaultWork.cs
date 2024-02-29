using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Event;
using Sixnet.Development.Domain.Event;
using Sixnet.Development.Event;
using Sixnet.Logging;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Default implements for work
    /// </summary>
    internal class DefaultWork : ISixnetWork
    {
        #region Constructor

        /// <summary>
        /// Initialize default work
        /// </summary>
        internal DefaultWork(IEnumerable<SixnetDatabaseServer> databaseServers = null, DataIsolationLevel? isolationLevel = null)
        {
            WorkId = Guid.NewGuid().ToString();
            dataClient = new DefaultDataClient(true, true, databaseServers, isolationLevel);
            UnitOfWork.TriggerCreateWorkEvent(this);
            UnitOfWork.Current = this;
        }

        #endregion

        #region Fields

        /// <summary>
        /// commit success event handler
        /// </summary>
        readonly ConcurrentQueue<Action<ISixnetWork>> commitSuccessEventHandlerCollection = new();

        /// <summary>
        /// domain events
        /// </summary>
        readonly ConcurrentQueue<ISixnetDomainEvent> domainEventCollection = new();

        /// <summary>
        /// data events
        /// </summary>
        readonly ConcurrentQueue<ISixnetDataEvent> dataEventCollection = new();

        /// <summary>
        /// Data client
        /// </summary>
        readonly ISixnetDataClient dataClient = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the work id
        /// </summary>
        public string WorkId { get; } = string.Empty;

        /// <summary>
        /// Get the data client
        /// </summary>
        public ISixnetDataClient DataClient => dataClient;

        #endregion

        #region Methods

        #region Commit

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            try
            {
                FrameworkLogManager.LogWorkStartSubmitting(this);

                // commit data client
                dataClient.Commit();

                // Data event
                HandleDataEvent();

                //Trigger work local commit success event
                TriggerCommitSuccessEvent();

                //Trigger work global commit success event
                UnitOfWork.TriggerWorkSuccessEvent(this);

                //Domain event
                HandleDomainEvent();

                FrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                return true;
            }
            catch (Exception ex)
            {
                UnitOfWork.TriggerWorkFailEvent(this);
                FrameworkLogManager.LogWorkSubmittedException(this, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                FrameworkLogManager.LogWorkStartSubmitting(this);

                // commit data client
                await dataClient.CommitAsync(cancellationToken).ConfigureAwait(false);

                // Data event
                await HandleDataEvent().ConfigureAwait(false);

                //Trigger work local commit success event
                TriggerCommitSuccessEvent();

                //Trigger work global commit success event
                UnitOfWork.TriggerWorkSuccessEvent(this);

                //Domain event
                await HandleDomainEvent(cancellationToken).ConfigureAwait(false);

                FrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                return true;
            }
            catch (Exception ex)
            {
                UnitOfWork.TriggerWorkFailEvent(this);
                FrameworkLogManager.LogWorkSubmittedException(this, ex);
                throw ex;
            }
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        public void Rollback()
        {
            FrameworkLogManager.LogWorkRollback(this);

            // rollback data client
            dataClient.Rollback();

            // rollback event
            UnitOfWork.TriggerWorkRollbackEvent(this);
        }

        #endregion

        #region Work event

        #region Subscribe commit success event

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public void SubscribeCommitSuccessEvent(params Action<ISixnetWork>[] eventHandlers)
        {
            IEnumerable<Action<ISixnetWork>> handlerCollection = eventHandlers;
            SubscribeCommitSuccessEvent(handlerCollection);
        }

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public void SubscribeCommitSuccessEvent(IEnumerable<Action<ISixnetWork>> eventHandlers)
        {
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    commitSuccessEventHandlerCollection.Enqueue(handler);
                }
            }
        }

        #endregion

        #region Trigger commit success event handler

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        void TriggerCommitSuccessEvent()
        {
            foreach (var handler in commitSuccessEventHandlerCollection)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(s => { eventHandler(this); });
            }
        }

        #endregion

        #endregion

        #region Domain event

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal void PublishDomainEvent(params ISixnetDomainEvent[] domainEvents)
        {
            IEnumerable<ISixnetDomainEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal void PublishDomainEvent(IEnumerable<ISixnetDomainEvent> domainEvents)
        {
            if (!domainEvents.IsNullOrEmpty())
            {
                foreach (var domainEvent in domainEvents)
                {
                    domainEventCollection.Enqueue(domainEvent);
                }
            }
        }

        /// <summary>
        /// Handle domain event
        /// </summary>
        Task HandleDomainEvent(CancellationToken cancellationToken = default)
        {
            var handleWorkTask = SixnetDomainEventBus.HandleWorkCompleted(domainEventCollection, cancellationToken);
            domainEventCollection?.Clear();
            return handleWorkTask;
        }

        #endregion

        #region Data event

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        internal void PublishDataEvent(params ISixnetDataEvent[] dataEvents)
        {
            IEnumerable<ISixnetDataEvent> eventCollection = dataEvents;
            PublishDataEvent(eventCollection);
        }

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        internal void PublishDataEvent(IEnumerable<ISixnetDataEvent> dataEvents)
        {
            if (!dataEvents.IsNullOrEmpty())
            {
                foreach (var dataEvent in dataEvents)
                {
                    dataEventCollection.Enqueue(dataEvent);
                }
            }
        }

        /// <summary>
        /// Handle data event
        /// </summary>
        Task HandleDataEvent(CancellationToken cancellationToken = default)
        {
            var handleWorkTask = SixnetDataEventBus.HandleWorkCompleted(dataEventCollection, cancellationToken);
            dataEventCollection?.Clear();
            return handleWorkTask;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            UnitOfWork.Current = null;
            FrameworkLogManager.LogWorkDispose(this);

            domainEventCollection?.Clear();
            commitSuccessEventHandlerCollection?.Clear();
            dataClient?.Dispose();
        }

        #endregion

        #endregion
    }
}
