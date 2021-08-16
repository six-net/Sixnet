using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Exceptions;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Data access event manager
    /// </summary>
    internal class DataAccessEventManager
    {
        /// <summary>
        /// Overall data access event handler collection
        /// </summary>
        readonly List<IDataAccessEventHandler> overallDataAccessEventHandlerCollection = new List<IDataAccessEventHandler>();

        /// <summary>
        /// Specific event handlers
        /// key:dataaccess event type id
        /// </summary>
        readonly Dictionary<Guid, List<IDataAccessEventHandler>> specificDataAccessEventHandlerCollection = new Dictionary<Guid, List<IDataAccessEventHandler>>();

        /// <summary>
        /// Entity overall data access event handlers
        /// </summary>
        readonly Dictionary<Guid, List<IDataAccessEventHandler>> entityOverallEventHandlerCollection = new Dictionary<Guid, List<IDataAccessEventHandler>>();

        /// <summary>
        /// Entity data access event handlers
        /// Key:entity type id->entity type id
        /// </summary>
        readonly Dictionary<Guid, Dictionary<Guid, List<IDataAccessEventHandler>>> entityDataAccessEventHandlerCollection = new Dictionary<Guid, Dictionary<Guid, List<IDataAccessEventHandler>>>();

        #region Publish

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataAccessEvents">Data access events</param>
        public void Publish(params IDataAccessEvent[] dataAccessEvents)
        {
            TriggerDataAccessEvent(dataAccessEvents);
        }

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataAccessEvents">Data access events</param>
        public void Publish(IEnumerable<IDataAccessEvent> dataAccessEvents)
        {
            TriggerDataAccessEvent(dataAccessEvents);
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeAll(IDataAccessEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                return;
            }
            overallDataAccessEventHandlerCollection.Add(eventHandler);
        }

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public void SubscribeAll(Func<IDataAccessEvent, DataAccessEventExecutionResult> eventHandleOperation)
        {
            IDataAccessEventHandler domainEventHandler = new DefaultDataAccessEventHandler<IDataAccessEvent>()
            {
                ExecuteEventOperation = eventHandleOperation
            };
            SubscribeAll(domainEventHandler);
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="eventHandler">Event handler</param>
        public void Subscribe(Type eventType, IDataAccessEventHandler eventHandler)
        {
            if (eventHandler == null || eventType == null)
            {
                return;
            }
            if (!typeof(IDataAccessEvent).IsAssignableFrom(eventType))
            {
                throw new EZNEWException($"{nameof(eventType.FullName)} not implementation {nameof(IDataAccessEvent)}");
            }
            if (!specificDataAccessEventHandlerCollection.TryGetValue(eventType.GUID, out List<IDataAccessEventHandler> eventHandlers) || eventHandlers == null)
            {
                eventHandlers = new List<IDataAccessEventHandler>();
            }
            eventHandlers.Add(eventHandler);
            specificDataAccessEventHandlerCollection[eventType.GUID] = eventHandlers;
        }

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public void Subscribe<TEvent>(IDataAccessEventHandler eventHandler) where TEvent : class, IDataAccessEvent
        {
            Subscribe(typeof(TEvent), eventHandler);
        }

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public void Subscribe<TEvent>(Func<TEvent, DataAccessEventExecutionResult> eventHandleOperation) where TEvent : class, IDataAccessEvent
        {
            IDataAccessEventHandler domainEventHandler = new DefaultDataAccessEventHandler<TEvent>()
            {
                ExecuteEventOperation = eventHandleOperation
            };
            Subscribe<TEvent>(domainEventHandler);
        }

        #endregion

        #region Entity overall

        /// <summary>
        /// Subscribe entity data access event in global area
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventHandler">handler</param>
        public void SubscribeEntityAll(Type entityType, IDataAccessEventHandler eventHandler)
        {
            if (entityType == null || eventHandler == null)
            {
                return;
            }
            if (!entityOverallEventHandlerCollection.TryGetValue(entityType.GUID, out var eventHandlers) || eventHandlers.IsNullOrEmpty())
            {
                eventHandlers = new List<IDataAccessEventHandler>();
            }
            eventHandlers.Add(eventHandler);
            entityOverallEventHandlerCollection[entityType.GUID] = eventHandlers;
        }

        /// <summary>
        /// Subscribe entity data access event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeEntityAll<T>(IDataAccessEventHandler eventHandler)
        {
            var entityType = typeof(T);
            SubscribeEntityAll(entityType, eventHandler);
        }

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public void SubscribeEntityAll<T>(Func<IDataAccessEvent, DataAccessEventExecutionResult> eventHandleOperation)
        {
            IDataAccessEventHandler domainEventHandler = new DefaultDataAccessEventHandler<IDataAccessEvent>()
            {
                ExecuteEventOperation = eventHandleOperation
            };
            SubscribeEntityAll<T>(domainEventHandler);
        }

        #endregion

        #region Entity specific event

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeEntity(Type entityType, Type eventType, IDataAccessEventHandler eventHandler)
        {
            if (entityType == null || eventType == null || eventHandler == null)
            {
                return;
            }
            if (!typeof(IDataAccessEvent).IsAssignableFrom(eventType))
            {
                throw new EZNEWException($"{nameof(eventType.FullName)} not implementation {nameof(IDataAccessEvent)}");
            }
            if (!entityDataAccessEventHandlerCollection.TryGetValue(entityType.GUID, out Dictionary<Guid, List<IDataAccessEventHandler>> entityEventHandlers) || entityEventHandlers == null)
            {
                entityEventHandlers = new Dictionary<Guid, List<IDataAccessEventHandler>>();
            }
            if (!entityEventHandlers.TryGetValue(eventType.GUID, out var eventHandlers) || eventHandlers.IsNullOrEmpty())
            {
                eventHandlers = new List<IDataAccessEventHandler>();
            }
            eventHandlers.Add(eventHandler);
            entityEventHandlers[eventType.GUID] = eventHandlers;
            entityDataAccessEventHandlerCollection[entityType.GUID] = entityEventHandlers;
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeEntity<TEvent>(Type entityType, IDataAccessEventHandler eventHandler)
        {
            if (entityType == null || eventHandler == null)
            {
                return;
            }
            var eventType = typeof(TEvent);
            SubscribeEntity(entityType, eventType, eventHandler);
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public void SubscribeEntity<TEntity, TEvent>(IDataAccessEventHandler eventHandler) where TEvent : class, IDataAccessEvent
        {
            SubscribeEntity<TEvent>(typeof(TEntity), eventHandler);
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public void SubscribeEntity<TEntity, TEvent>(Func<TEvent, DataAccessEventExecutionResult> eventHandleOperation) where TEvent : class, IDataAccessEvent
        {
            IDataAccessEventHandler domainEventHandler = new DefaultDataAccessEventHandler<TEvent>()
            {
                ExecuteEventOperation = eventHandleOperation
            };
            SubscribeEntity<TEntity, TEvent>(domainEventHandler);
        }

        #endregion

        #endregion

        #region Trigger event

        /// <summary>
        /// Trigger data access event
        /// </summary>
        /// <param name="dataAccessEvents">Data access events</param>
        /// <returns></returns>
        void TriggerDataAccessEvent(IEnumerable<IDataAccessEvent> dataAccessEvents)
        {
            if (dataAccessEvents.IsNullOrEmpty())
            {
                return;
            }
            foreach (var dataAccessEvent in dataAccessEvents)
            {
                //overall
                foreach (var overallHandler in overallDataAccessEventHandlerCollection)
                {
                    var overallEventHandler = overallHandler;
                    ThreadPool.QueueUserWorkItem(s => { overallEventHandler.Execute(dataAccessEvent); });
                }
                //specific data access event
                var eventType = dataAccessEvent.GetType();
                if (specificDataAccessEventHandlerCollection.TryGetValue(eventType.GUID, out var specificEventHandlers) && !specificEventHandlers.IsNullOrEmpty())
                {
                    foreach (var specificHandler in specificEventHandlers)
                    {
                        var specificEventHandler = specificHandler;
                        ThreadPool.QueueUserWorkItem(s => { specificEventHandler.Execute(dataAccessEvent); });
                    }
                }

                var entityType = dataAccessEvent.EntityType;
                if (entityType == null)
                {
                    continue;
                }

                //entity overall
                if (entityOverallEventHandlerCollection.TryGetValue(entityType.GUID, out var entityOverallHandlers) && !entityOverallHandlers.IsNullOrEmpty())
                {
                    foreach (var entityOverallHandler in entityOverallHandlers)
                    {
                        var entityOverallEventHandler = entityOverallHandler;
                        ThreadPool.QueueUserWorkItem(s => { entityOverallEventHandler.Execute(dataAccessEvent); });
                    }
                }
                //entity data access event
                if (entityDataAccessEventHandlerCollection.TryGetValue(entityType.GUID, out var entityHandlerCollection) && !entityHandlerCollection.IsNullOrEmpty())
                {
                    if (entityHandlerCollection.TryGetValue(eventType.GUID, out var entityEventHandlers) && !entityEventHandlers.IsNullOrEmpty())
                    {
                        foreach (var entitySpecificHandler in entityEventHandlers)
                        {
                            var entitySpecificEventHandler = entitySpecificHandler;
                            ThreadPool.QueueUserWorkItem(s => { entitySpecificEventHandler.Execute(dataAccessEvent); });
                        }
                    }
                }
            }
        }

        #endregion
    }
}
