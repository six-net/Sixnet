using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// Domain event bus
    /// </summary>
    public static class DomainEventBus
    {
        /// <summary>
        /// Global domain event manager
        /// </summary>
        internal static DomainEventManager GlobalDomainEventManager = null;

        static DomainEventBus()
        {
            GlobalDomainEventManager = new DomainEventManager();
        }

        #region Publish

        /// <summary>
        /// Publish domain events
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        public static void Publish(params IDomainEvent[] domainEvents)
        {
            if (domainEvents.IsNullOrEmpty())
            {
                return;
            }
            GlobalDomainEventManager.Publish(domainEvents);
            if (WorkManager.Current != null)
            {
                WorkManager.Current.PublishDomainEvent(domainEvents);
            }
        }

        #endregion

        #region Subscribe

        #region Global event

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public static void GlobalSubscribe<TEvent>(Func<TEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately) where TEvent : class, IDomainEvent
        {
            GlobalDomainEventManager?.Subscribe(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public static void GlobalSubscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
        {
            GlobalDomainEventManager?.Subscribe<TEvent>(eventHandler);
        }

        #endregion

        #region Global all event

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public static void GlobalSubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately)
        {
            GlobalDomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public static void GlobalSubscribeAll(IDomainEventHandler eventHandler)
        {
            GlobalDomainEventManager?.SubscribeAll(eventHandler);
        }

        #endregion

        #region Work event

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public static void WorkSubscribe<TEvent>(Func<TEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately) where TEvent : class, IDomainEvent
        {
            WorkManager.Current?.DomainEventManager?.Subscribe(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public static void WorkSubscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
        {
            WorkManager.Current?.DomainEventManager?.Subscribe<TEvent>(eventHandler);
        }

        #endregion

        #region Work all event

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public static void WorkSubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately)
        {
            WorkManager.Current?.DomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public static void WorkSubscribeAll(IDomainEventHandler eventHandler)
        {
            WorkManager.Current?.DomainEventManager?.SubscribeAll(eventHandler);
        }

        #endregion

        #endregion
    }
}
