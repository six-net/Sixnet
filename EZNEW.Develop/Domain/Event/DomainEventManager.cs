using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// domain event manager
    /// </summary>
    public class DomainEventManager
    {
        /// <summary>
        /// global event handlers
        /// </summary>
        Dictionary<Guid, Dictionary<EventExecuteTime, List<IDomainEventHandler>>> eventHandlerCollection = new Dictionary<Guid, Dictionary<EventExecuteTime, List<IDomainEventHandler>>>();

        /// <summary>
        /// overall event handler collection
        /// </summary>
        Dictionary<EventExecuteTime, List<IDomainEventHandler>> overallEventHandlerCollection = new Dictionary<EventExecuteTime, List<IDomainEventHandler>>();

        #region publish

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        public void Publish(params IDomainEvent[] domainEvents)
        {
            PublishAsync(domainEvents).Wait();
        }

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        public async Task PublishAsync(params IDomainEvent[] domainEvents)
        {
            await ExecutedTimeDomainEventAsync(EventExecuteTime.Immediately, domainEvents).ConfigureAwait(false);
        }

        #endregion

        #region execute event

        /// <summary>
        /// executed domain event
        /// </summary>
        /// <param name="eventExecuteTime">event time</param>
        /// <param name="domainEvents">domain events</param>
        internal void ExecutedTimeDomainEvent(EventExecuteTime eventExecuteTime, params IDomainEvent[] domainEvents)
        {
            ExecutedTimeDomainEventAsync(eventExecuteTime, domainEvents).Wait();
        }

        /// <summary>
        /// executed domain event
        /// </summary>
        /// <param name="eventExecuteTime">event time</param>
        /// <param name="domainEvents">domain events</param>
        internal async Task ExecutedTimeDomainEventAsync(EventExecuteTime eventExecuteTime, params IDomainEvent[] domainEvents)
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
                if (overallEventHandlerCollection.TryGetValue(eventExecuteTime, out var overallHandlers) && !overallHandlers.IsNullOrEmpty())
                {
                    foreach (var overallHandler in overallHandlers)
                    {
                        await overallHandler.ExecuteAsync(domainEvent).ConfigureAwait(false);
                    }
                }

                //spectial event handler
                var eventType = domainEvent.GetType();
                if (!eventHandlerCollection.TryGetValue(eventType.GUID, out var eventTimeHandlers) || eventTimeHandlers.IsNullOrEmpty())
                {
                    continue;
                }
                if (!eventTimeHandlers.TryGetValue(eventExecuteTime, out var handlers) || handlers.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var handler in handlers)
                {
                    await handler.ExecuteAsync(domainEvent).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region subscribe spectial event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public void Subscribe<Event>(Func<Event, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            if (eventHandleOperation == null)
            {
                return;
            }
            Func<Event, Task<DomainEventExecuteResult>> asyncOperation = x => Task.FromResult(eventHandleOperation(x));
            Subscribe(asyncOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public void Subscribe<Event>(Func<Event, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately) where Event : class, IDomainEvent
        {
            IDomainEventHandler domainEventHandler = null;
            switch (executeTime)
            {
                case EventExecuteTime.Immediately:
                    domainEventHandler = new DefaultImmediatelyDomainEventHandler<Event>()
                    {
                        ExecuteEventOperation = eventHandleOperationAsync
                    };
                    break;
                case EventExecuteTime.WorkCompleted:
                    domainEventHandler = new DefaultWorkCompletedDomainEventHandler<Event>()
                    {
                        ExecuteEventOperation = eventHandleOperationAsync
                    };
                    break;
            }
            Subscribe<Event>(domainEventHandler);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="handler"></param>
        public void Subscribe<Event>(IDomainEventHandler handler) where Event : class, IDomainEvent
        {
            if (handler == null)
            {
                return;
            }
            var eventType = typeof(Event);
            if (!eventHandlerCollection.TryGetValue(eventType.GUID, out Dictionary<EventExecuteTime, List<IDomainEventHandler>> executeTimeHandlers) || executeTimeHandlers == null)
            {
                executeTimeHandlers = new Dictionary<EventExecuteTime, List<IDomainEventHandler>>()
                {
                    { handler.ExecuteTime,new List<IDomainEventHandler>(){ handler } }
                };
            }
            else
            {
                if (!executeTimeHandlers.TryGetValue(handler.ExecuteTime, out var handlers) || handlers == null)
                {
                    executeTimeHandlers[handler.ExecuteTime] = new List<IDomainEventHandler>() { handler };
                }
                else
                {
                    handlers.Add(handler);
                }
            }
            eventHandlerCollection[eventType.GUID] = executeTimeHandlers;
        }

        #endregion

        #region subscribe overall event

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="eventHandleOperation">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public void SubscribeAll(Func<IDomainEvent, DomainEventExecuteResult> eventHandleOperation, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            if (eventHandleOperation == null)
            {
                return;
            }
            Func<IDomainEvent, Task<DomainEventExecuteResult>> asyncOperation = x => Task.FromResult(eventHandleOperation(x));
            SubscribeAll(asyncOperation, executeTime);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="eventHandleOperationAsync">event handle operation</param>
        /// <param name="executeTime">execute time</param>
        public void SubscribeAll(Func<IDomainEvent, Task<DomainEventExecuteResult>> eventHandleOperationAsync, EventExecuteTime executeTime = EventExecuteTime.Immediately)
        {
            IDomainEventHandler domainEventHandler = null;
            switch (executeTime)
            {
                case EventExecuteTime.Immediately:
                    domainEventHandler = new DefaultImmediatelyDomainEventHandler<IDomainEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperationAsync
                    };
                    break;
                case EventExecuteTime.WorkCompleted:
                    domainEventHandler = new DefaultWorkCompletedDomainEventHandler<IDomainEvent>()
                    {
                        ExecuteEventOperation = eventHandleOperationAsync
                    };
                    break;
            }
            SubscribeAll(domainEventHandler);
        }

        /// <summary>
        /// subscribe domain event in global area
        /// </summary>
        /// <typeparam name="Event">event</typeparam>
        /// <param name="handler"></param>
        public void SubscribeAll(IDomainEventHandler handler)
        {
            if (!overallEventHandlerCollection.TryGetValue(handler.ExecuteTime, out var handlers) || handlers == null)
            {
                handlers = new List<IDomainEventHandler>() { handler };
            }
            else
            {
                handlers.Add(handler);
            }
            overallEventHandlerCollection[handler.ExecuteTime] = handlers;
        }

        #endregion
    }
}
