using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Domain event manager
    /// </summary>
    public class DomainEventManager
    {
        /// <summary>
        /// Global event handlers
        /// </summary>
        readonly Dictionary<Guid, Dictionary<EventTriggerTime, List<IDomainEventHandler>>> eventHandlerCollection = new Dictionary<Guid, Dictionary<EventTriggerTime, List<IDomainEventHandler>>>();

        /// <summary>
        /// Overall event handler collection
        /// </summary>
        readonly Dictionary<EventTriggerTime, List<IDomainEventHandler>> overallEventHandlerCollection = new Dictionary<EventTriggerTime, List<IDomainEventHandler>>();

        #region Publish

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        public void Publish(params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> eventCollection = domainEvents;
            Publish(eventCollection);
        }

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        public void Publish(IEnumerable<IDomainEvent> domainEvents)
        {
            TriggerDomainEvent(EventTriggerTime.Immediately, domainEvents);
        }

        #endregion

        #region Subscribe spectial event

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public void Subscribe<TEvent>(Func<TEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately) where TEvent : class, IDomainEvent
        {
            IDomainEventHandler domainEventHandler = null;
            switch (executeTime)
            {
                case EventTriggerTime.Immediately:
                    domainEventHandler = new DefaultImmediatelyDomainEventHandler<TEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperation
                    };
                    break;
                case EventTriggerTime.WorkCompleted:
                    domainEventHandler = new DefaultWorkCompletedDomainEventHandler<TEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperation
                    };
                    break;
            }
            Subscribe<TEvent>(domainEventHandler);
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Domain event</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public void Subscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
        {
            if (eventHandler == null)
            {
                return;
            }
            var eventType = typeof(TEvent);
            if (!eventHandlerCollection.TryGetValue(eventType.GUID, out Dictionary<EventTriggerTime, List<IDomainEventHandler>> executeTimeHandlers) || executeTimeHandlers == null)
            {
                executeTimeHandlers = new Dictionary<EventTriggerTime, List<IDomainEventHandler>>()
                {
                    { eventHandler.ExecuteTime,new List<IDomainEventHandler>(){ eventHandler } }
                };
            }
            else
            {
                if (!executeTimeHandlers.TryGetValue(eventHandler.ExecuteTime, out var handlers) || handlers == null)
                {
                    executeTimeHandlers[eventHandler.ExecuteTime] = new List<IDomainEventHandler>() { eventHandler };
                }
                else
                {
                    handlers.Add(eventHandler);
                }
            }
            eventHandlerCollection[eventType.GUID] = executeTimeHandlers;
        }

        #endregion

        #region Subscribe overall event

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        /// <param name="executeTime">Execute time</param>
        public void SubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventTriggerTime executeTime = EventTriggerTime.Immediately)
        {
            IDomainEventHandler domainEventHandler = null;
            switch (executeTime)
            {
                case EventTriggerTime.Immediately:
                    domainEventHandler = new DefaultImmediatelyDomainEventHandler<IDomainEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperation
                    };
                    break;
                case EventTriggerTime.WorkCompleted:
                    domainEventHandler = new DefaultWorkCompletedDomainEventHandler<IDomainEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperation
                    };
                    break;
            }
            SubscribeAll(domainEventHandler);
        }

        /// <summary>
        /// Subscribe domain event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeAll(IDomainEventHandler eventHandler)
        {
            if (!overallEventHandlerCollection.TryGetValue(eventHandler.ExecuteTime, out var handlers) || handlers == null)
            {
                handlers = new List<IDomainEventHandler>() { eventHandler };
            }
            else
            {
                handlers.Add(eventHandler);
            }
            overallEventHandlerCollection[eventHandler.ExecuteTime] = handlers;
        }

        #endregion

        #region Trigger event

        /// <summary>
        /// Trigger domain event
        /// </summary>
        /// <param name="triggerTime">Trigger time</param>
        /// <param name="domainEvents">Domain events</param>
        internal void TriggerDomainEvent(EventTriggerTime triggerTime, params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> eventCollection = domainEvents;
            TriggerDomainEvent(triggerTime, eventCollection);
        }

        /// <summary>
        /// Trigger domain event
        /// </summary>
        /// <param name="triggerTime">Trigger time</param>
        /// <param name="domainEvents">Domain events</param>
        internal void TriggerDomainEvent(EventTriggerTime triggerTime, IEnumerable<IDomainEvent> domainEvents)
        {
            if (domainEvents.IsNullOrEmpty())
            {
                return;
            }
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent == null)
                {
                    continue;
                }
                //overall event handler
                overallEventHandlerCollection.TryGetValue(triggerTime, out var allHandlers);

                //spectial event handler
                var eventType = domainEvent.GetType();
                if (eventHandlerCollection.TryGetValue(eventType.GUID, out var eventTimeHandlers) && !eventTimeHandlers.IsNullOrEmpty() && eventTimeHandlers.TryGetValue(triggerTime, out var handlers) && !handlers.IsNullOrEmpty())
                {
                    if (allHandlers == null)
                    {
                        allHandlers = handlers;
                    }
                    else
                    {
                        allHandlers.AddRange(handlers);
                    }
                }
                if (allHandlers.IsNullOrEmpty())
                {
                    continue;
                }
                if (triggerTime == EventTriggerTime.Immediately)
                {
                    foreach (var handler in allHandlers)
                    {
                        handler.Execute(domainEvent);
                    }
                }
                else
                {
                    foreach (var handler in allHandlers)
                    {
                        var domainEventHandler = handler;
                        ThreadPool.QueueUserWorkItem(s => { domainEventHandler.Execute(domainEvent); });
                    }
                }
            }
        }

        #endregion

        #region Reset

        /// <summary>
        /// Reset domain event manager
        /// Clear all event handler
        /// </summary>
        public void Reset()
        {
            eventHandlerCollection.Clear();
            overallEventHandlerCollection.Clear();
        }

        #endregion
    }
}
