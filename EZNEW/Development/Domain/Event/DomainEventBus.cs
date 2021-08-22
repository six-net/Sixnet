using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.UnitOfWork;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Domain event bus
    /// </summary>
    public static class DomainEventBus
    {
        /// <summary>
        /// Domain event manager
        /// </summary>
        internal static DomainEventManager DomainEventManager = null;

        static DomainEventBus()
        {
            DomainEventManager = new DomainEventManager();
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
            DomainEventManager.Publish(domainEvents);
            if (WorkManager.Current != null)
            {
                WorkManager.Current.PublishDomainEvent(domainEvents);
            }
        }

        #endregion

        #region Subscribe

        #region Global event

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Trigger time</param>
        public static void Subscribe<TEvent>(Func<TEvent, DomainEventResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately) where TEvent : class, IDomainEvent
        {
            DomainEventManager?.Subscribe(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public static void Subscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
        {
            DomainEventManager?.Subscribe<TEvent>(eventHandler);
        }

        #endregion

        #region Global all event

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Trigger time</param>
        public static void SubscribeAll(Func<IDomainEvent, DomainEventResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately)
        {
            DomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public static void SubscribeAll(IDomainEventHandler eventHandler)
        {
            DomainEventManager?.SubscribeAll(eventHandler);
        }

        #endregion

        #region Work event

        /// <summary>
        /// Subscribe domain event in work area
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Trigger time</param>
        public static void WorkSubscribe<TEvent>(Func<TEvent, DomainEventResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately) where TEvent : class, IDomainEvent
        {
            WorkManager.Current?.DomainEventManager?.Subscribe(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event
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
        /// Subscribe domain event in work area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Trigger time</param>
        public static void WorkSubscribeAll(Func<IDomainEvent, DomainEventResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately)
        {
            WorkManager.Current?.DomainEventManager?.SubscribeAll(eventHandleOperation, executeTime);
        }

        /// <summary>
        /// Subscribe domain event in work area
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
