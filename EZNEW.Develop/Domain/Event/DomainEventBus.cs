using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// domain event bus
    /// </summary>
    public static class DomainEventBus
    {
        internal static DomainEventManager globalDomainEventManager = null;

        static DomainEventBus()
        {
            globalDomainEventManager = new DomainEventManager();
        }

        #region publish

        /// <summary>
        /// publish domain events
        /// </summary>
        /// <param name="domainEvents">domain events</param>
        public static void Publish(params IDomainEvent[] domainEvents)
        {
            PublishAsync(domainEvents).Wait();
        }

        /// <summary>
        /// publish domain events
        /// </summary>
        /// <param name="domainEvents">domain events</param>
        public static async Task PublishAsync(params IDomainEvent[] domainEvents)
        {
            if (domainEvents.IsNullOrEmpty())
            {
                return;
            }
            await globalDomainEventManager.PublishAsync(domainEvents).ConfigureAwait(false);
            if (WorkFactory.Current != null)
            {
                await WorkFactory.Current.PublishDomainEventAsync(domainEvents).ConfigureAwait(false);
            }
        }

        #endregion

        #region subscribe

        #region global event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void GlobalSubscribe<Event>(Func<Event, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            globalDomainEventManager?.Subscribe(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void GlobalSubscribe<Event>(Func<Event, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            globalDomainEventManager?.Subscribe(eventHandleOperationAsync, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="handler"></param>
        public static void GlobalSubscribe<Event>(IDomainEventHandler handler) where Event : class, IDomainEvent
        {
            globalDomainEventManager?.Subscribe<Event>(handler);
        }

        #endregion

        #region global all event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void GlobalSubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            globalDomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void GlobalSubscribeAll(Func<IDomainEvent, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            globalDomainEventManager?.SubscribeAll(eventHandleOperationAsync, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="handler"></param>
        public static void GlobalSubscribeAll(IDomainEventHandler handler)
        {
            globalDomainEventManager?.SubscribeAll(handler);
        }

        #endregion

        #region work event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void WorkSubscribe<Event>(Func<Event, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            WorkFactory.Current?.DomainEventManager?.Subscribe<Event>(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void WorkSubscribe<Event>(Func<Event, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            WorkFactory.Current?.DomainEventManager?.Subscribe<Event>(eventHandleOperationAsync, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="handler"></param>
        public static void WorkSubscribe<Event>(IDomainEventHandler handler) where Event : class, IDomainEvent
        {
            WorkFactory.Current?.DomainEventManager?.Subscribe<Event>(handler);
        }

        #endregion

        #region work all event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void WorkSubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            WorkFactory.Current?.DomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public static void WorkSubscribeAll(Func<IDomainEvent, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            WorkFactory.Current?.DomainEventManager?.SubscribeAll(eventHandleOperationAsync, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <param name="handler"></param>
        public static void WorkSubscribeAll(IDomainEventHandler handler)
        {
            WorkFactory.Current?.DomainEventManager?.SubscribeAll(handler);
        }

        #endregion

        #endregion
    }
}
