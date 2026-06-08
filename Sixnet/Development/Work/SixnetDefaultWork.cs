// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Events;
using Sixnet.Logging;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Default implements for work
    /// </summary>
    internal class SixnetDefaultWork : ISixnetWork
    {
        #region Constructor

        /// <summary>
        /// Initialize default work
        /// </summary>
        internal SixnetDefaultWork(IEnumerable<SixnetDatabaseServer> databaseServers = null, SixnetDataIsolationLevel? isolationLevel = null)
        {
            WorkId = Guid.NewGuid().ToString();
            dataClient = new SixnetDefaultDataClient(true, true, databaseServers, isolationLevel);
            SixnetUnitOfWork.TriggerCreateWorkEvent(this);
            SixnetUnitOfWork.Current = this;
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
        readonly ConcurrentQueue<ISixnetEvent> domainEvents = new();

        /// <summary>
        /// data events
        /// </summary>
        readonly ConcurrentQueue<ISixnetEvent> dataEvents = new();

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
                SixnetFrameworkLogManager.LogWorkStartSubmitting(this);

                dataClient.Commit();

                SixnetFrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                success = true;
            }
            catch (Exception ex)
            {
                SixnetFrameworkLogManager.LogWorkSubmittedException(this, ex);
                success = false;
                throw;
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
                SixnetFrameworkLogManager.LogWorkStartSubmitting(this);

                await dataClient.CommitAsync(cancellationToken).ConfigureAwait(false);

                SixnetFrameworkLogManager.LogWorkSubmittedSuccessfully(this);

                success = true;
            }
            catch (Exception ex)
            {
                SixnetFrameworkLogManager.LogWorkSubmittedException(this, ex);
                success = false;
                throw;
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
            var workDataEvents = new List<ISixnetEvent>(dataEvents);
            var workDomainEvents = new List<ISixnetEvent>(domainEvents);
            ThreadPool.QueueUserWorkItem(s =>
            {
                var work = s as ISixnetWork;

                // clear unit work context
                SixnetUnitOfWork.Current = null;

                // Data event
                SixnetEventBus.PublishWorkCompletedEventAsync(workDataEvents);

                //Domain event
                SixnetEventBus.PublishWorkCompletedEventAsync(workDomainEvents);

                //Trigger work local commit success event
                TriggerCommitSuccessEvent(work);

                //Trigger work global commit success event
                SixnetUnitOfWork.TriggerWorkSuccessEvent(work);

            }, this);
        }

        /// <summary>
        /// Trigger fail event
        /// </summary>
        void TriggerFailEvent()
        {
            SixnetUnitOfWork.TriggerWorkFailEvent(this);
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
            SixnetFrameworkLogManager.LogWorkRollback(this);

            // rollback data client
            dataClient.Rollback();

            // rollback event
            SixnetUnitOfWork.TriggerWorkRollbackEvent(this);
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
        internal void PublishDomainEvent(params ISixnetEvent[] domainEvents)
        {
            IEnumerable<ISixnetEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        internal void PublishDomainEvent(IEnumerable<ISixnetEvent> domainEvents)
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
        internal void PublishDataEvent(params ISixnetEvent[] dataEvents)
        {
            IEnumerable<ISixnetEvent> eventCollection = dataEvents;
            PublishDataEvent(eventCollection);
        }

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        internal void PublishDataEvent(IEnumerable<ISixnetEvent> dataEvents)
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
            SixnetUnitOfWork.Current = null;
            dataClient?.Dispose();

            SixnetFrameworkLogManager.LogWorkDispose(this);

            ClearEvent();
            commitSuccessEventHandlers?.Clear();
        }

        #endregion

        #endregion
    }
}
