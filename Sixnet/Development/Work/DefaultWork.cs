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
        internal DefaultWork(IEnumerable<DatabaseServer> databaseServers = null, DataIsolationLevel? isolationLevel = null)
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
        readonly ConcurrentQueue<Action<ISixnetWork>> commitSuccessEventHandlers = new();

        /// <summary>
        /// domain events
        /// </summary>
        readonly ConcurrentQueue<ISixnetDomainEvent> domainEvents = new();

        /// <summary>
        /// data events
        /// </summary>
        readonly ConcurrentQueue<ISixnetDataEvent> dataEvents = new();

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
            var success = false;
            try
            {
                FrameworkLogManager.LogWorkStartSubmitting(this);

                dataClient.Commit();

                FrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                success = true;
            }
            catch (Exception ex)
            {
                FrameworkLogManager.LogWorkSubmittedException(this, ex);
                success = false;
                throw ex;
            }
            finally
            {
                TriggerWorkEvent(success);
            }
            return success;
        }

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            var success = false;
            try
            {
                FrameworkLogManager.LogWorkStartSubmitting(this);

                await dataClient.CommitAsync(cancellationToken).ConfigureAwait(false);

                FrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                success = true;
            }
            catch (Exception ex)
            {
                FrameworkLogManager.LogWorkSubmittedException(this, ex);
                success = false;
                throw ex;
            }
            finally
            {
                TriggerWorkEvent(success);
            }
            return success;
        }

        /// <summary>
        /// Trigger success event
        /// </summary>
        /// <returns></returns>
        void TriggerSuccessEvent()
        {
            var workDataEvents = new List<ISixnetDataEvent>(dataEvents);
            var workDomainEvents = new List<ISixnetDomainEvent>(domainEvents);
            ThreadPool.QueueUserWorkItem(s =>
            {
                ISixnetWork work = s as ISixnetWork;

                // clear unit work context
                UnitOfWork.Current = null;

                // Data event
                SixnetDataEventBus.PublishWorkCompletedEventAsync(workDataEvents);

                //Domain event
                SixnetDomainEventBus.PublishWorkCompletedEventAsync(workDomainEvents);

                //Trigger work local commit success event
                TriggerCommitSuccessEvent(work);

                //Trigger work global commit success event
                UnitOfWork.TriggerWorkSuccessEvent(work);

            }, this);
        }

        /// <summary>
        /// Trigger fail event
        /// </summary>
        void TriggerFailEvent()
        {
            UnitOfWork.TriggerWorkFailEvent(this);
        }

        /// <summary>
        /// Trigger work event
        /// </summary>
        /// <param name="success"></param>
        void TriggerWorkEvent(bool success)
        {
            if (success)
            {
                TriggerSuccessEvent();
            }
            else
            {
                TriggerFailEvent();
            }
            ClearEvent();
        }

        /// <summary>
        /// Clear event
        /// </summary>
        void ClearEvent()
        {
            domainEvents?.Clear();
            dataEvents?.Clear();
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
                    commitSuccessEventHandlers.Enqueue(handler);
                }
            }
        }

        #endregion

        #region Trigger commit success event handler

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        void TriggerCommitSuccessEvent(ISixnetWork work)
        {
            foreach (var handler in commitSuccessEventHandlers)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(s => { eventHandler(work); });
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
                    this.domainEvents.Enqueue(domainEvent);
                }
            }
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
                    this.dataEvents.Enqueue(dataEvent);
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            UnitOfWork.Current = null;
            dataClient?.Dispose();

            FrameworkLogManager.LogWorkDispose(this);

            ClearEvent();
            commitSuccessEventHandlers?.Clear();
        }

        #endregion

        #endregion
    }
}
