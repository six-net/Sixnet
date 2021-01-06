using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;
using EZNEW.DependencyInjection;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Repository event bus
    /// </summary>
    public static class RepositoryEventBus
    {
        /// <summary>
        /// Key:source repository->operation data->event type
        /// </summary>
        static readonly Dictionary<Guid, Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>> EventWarehouses = new Dictionary<Guid, Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>>();

        #region Subscribe

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <param name="eventSource">Event source</param>
        /// <param name="repositoryEventHandler">Event handler</param>
        public static void Subscribe(Type eventSource, IRepositoryEventHandler repositoryEventHandler)
        {
            if (eventSource == null || repositoryEventHandler == null || repositoryEventHandler.ObjectType == null || repositoryEventHandler.HandlerRepositoryType == null)
            {
                return;
            }
            var eventSourceId = eventSource.GUID;
            var objectId = repositoryEventHandler.ObjectType.GUID;
            if (!EventWarehouses.TryGetValue(eventSourceId, out var sourceEvents) || sourceEvents.IsNullOrEmpty())
            {
                sourceEvents = new Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>();
                EventWarehouses[eventSourceId] = sourceEvents;
            }
            if (!sourceEvents.TryGetValue(objectId, out var eventDict) || eventDict.IsNullOrEmpty())
            {
                eventDict = new Dictionary<EventType, List<IRepositoryEventHandler>>();
                sourceEvents[objectId] = eventDict;
            }
            if (!eventDict.TryGetValue(repositoryEventHandler.EventType, out var dataEvents) || dataEvents.IsNullOrEmpty())
            {
                dataEvents = new List<IRepositoryEventHandler>();
                eventDict[repositoryEventHandler.EventType] = dataEvents;
            }
            if (!dataEvents.Exists(c => c.HandlerRepositoryType == repositoryEventHandler.HandlerRepositoryType))
            {
                dataEvents.Add(repositoryEventHandler);
            }
        }

        #region Data operation

        /// <summary>
        /// Subscribe data operation
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="eventSourceType">Event source type</param>
        /// <param name="objectType">Data object type</param>
        /// <param name="eventHandlerType">Event handler type</param>
        /// <param name="actionName">Action name</param>
        public static void SubscribeDataOperation(EventType eventType, Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || string.IsNullOrWhiteSpace(actionName))
            {
                return;
            }
            var eventSource = ContainerManager.Resolve(eventSourceType);
            var eventHandler = ContainerManager.Resolve(eventHandlerType);
            if (eventSource == null || eventHandler == null)
            {
                return;
            }
            var parameterType = typeof(IEnumerable<>).MakeGenericType(objectType);
            var activationOptionType = typeof(ActivationOptions);
            var actionMember = eventHandler.GetType().GetMethod(actionName, new Type[] { parameterType, activationOptionType });
            if (actionMember == null)
            {
                return;
            }
            var handlerAction = Delegate.CreateDelegate(typeof(DataOperation<>).MakeGenericType(objectType), eventHandler, actionMember);
            if (handlerAction == null)
            {
                return;
            }
            var dataOperationEventHandlerType = typeof(DataEventHandler<>).MakeGenericType(objectType);
            var dataOperationEventHandler = (IRepositoryEventHandler)Activator.CreateInstance(dataOperationEventHandlerType);
            var eventProperty = dataOperationEventHandlerType.GetProperty("EventType");
            eventProperty.SetValue(dataOperationEventHandler, eventType);
            var handlerRepositoryTypeProperty = dataOperationEventHandlerType.GetProperty("HandlerRepositoryType");
            handlerRepositoryTypeProperty.SetValue(dataOperationEventHandler, eventHandler.GetType());
            var objectTypeProperty = dataOperationEventHandlerType.GetProperty("ObjectType");
            objectTypeProperty.SetValue(dataOperationEventHandler, objectType);
            var operationProperty = dataOperationEventHandlerType.GetProperty("Operation");
            operationProperty.SetValue(dataOperationEventHandler, handlerAction);

            Subscribe(eventSource.GetType(), dataOperationEventHandler);
        }

        /// <summary>
        /// Subscribe data operation repository event
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventType">Event type</param>
        /// <param name="action">Action</param>
        public static void SubscribeDataOperation<TEventSource, TEventHandler, TObject>(EventType eventType, Expression<Func<TEventHandler, DataOperation<TObject>>> action)
        {
            SubscribeDataOperation(eventType, typeof(TEventSource), typeof(TObject), typeof(TEventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        /// <summary>
        /// Subscribe save event
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeSave<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, DataOperation<TObject>>> action)
        {
            SubscribeDataOperation<TEventSource, TEventHandler, TObject>(EventType.SaveObject, action);
        }

        /// <summary>
        /// Subscribe remove event
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeObjectRemove<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, DataOperation<TObject>>> action)
        {
            SubscribeDataOperation<TEventSource, TEventHandler, TObject>(EventType.RemoveObject, action);
        }

        /// <summary>
        /// Subscribe remove(both object and condition)
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeRemove<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, DataOperation<TObject>>> objectRemoveAction = null, Expression<Func<TEventHandler, ConditionOperation>> conditionRemoveAction = null)
        {
            //object remove
            if (objectRemoveAction != null)
            {
                SubscribeObjectRemove<TEventSource, TEventHandler, TObject>(objectRemoveAction);
            }
            //condition remove
            if (conditionRemoveAction != null)
            {
                SubscribeConditionRemove<TEventSource, TEventHandler, TObject>(conditionRemoveAction);
            }
        }

        #endregion

        #region Condition operation

        /// <summary>
        /// Subscribe condition remove
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="eventSourceType">Event source type</param>
        /// <param name="objectType">Event object type</param>
        /// <param name="eventHandlerType">Event handler type</param>
        /// <param name="actionName">Action name</param>
        public static void SubscribeConditionOperation(EventType eventType, Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || string.IsNullOrWhiteSpace(actionName))
            {
                return;
            }
            var eventSource = ContainerManager.Resolve(eventSourceType);
            var eventHandler = ContainerManager.Resolve(eventHandlerType);
            if (eventSource == null || eventHandler == null)
            {
                return;
            }
            var actionMember = eventHandler.GetType().GetMethod(actionName, new Type[] { typeof(IQuery), typeof(ActivationOptions) });
            if (actionMember == null)
            {
                return;
            }
            var handlerAction = Delegate.CreateDelegate(typeof(ConditionOperation), eventHandler, actionMember);
            if (handlerAction == null)
            {
                return;
            }
            var conditionEventHandler = new ConditionEventHandler()
            {
                EventType = eventType,
                HandlerRepositoryType = eventHandler.GetType(),
                ObjectType = objectType,
                Operation = (ConditionOperation)handlerAction
            };
            Subscribe(eventSource.GetType(), conditionEventHandler);
        }

        /// <summary>
        /// Subscribe condition remove
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventType">Event type</param>
        /// <param name="action">Action</param>
        public static void SubscribeConditionOperation<TEventSource, TEventHandler, TObject>(EventType eventType, Expression<Func<TEventHandler, ConditionOperation>> action)
        {
            SubscribeConditionOperation(eventType, typeof(TEventSource), typeof(TObject), typeof(TEventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        /// <summary>
        /// Subscribe save event
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeConditionRemove<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, ConditionOperation>> action)
        {
            SubscribeConditionOperation<TEventSource, TEventHandler, TObject>(EventType.RemoveByCondition, action);
        }

        #endregion

        #region Modify

        /// <summary>
        /// Subscribe condition remove
        /// </summary>
        /// <param name="eventSourceType">Event source type</param>
        /// <param name="objectType">Object type</param>
        /// <param name="eventHandlerType">Event handler type</param>
        /// <param name="actionName">Action name</param>
        public static void SubscribeModify(Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || string.IsNullOrWhiteSpace(actionName))
            {
                return;
            }
            var eventSource = ContainerManager.Resolve(eventSourceType);
            var eventHandler = ContainerManager.Resolve(eventHandlerType);
            if (eventSource == null || eventHandler == null)
            {
                return;
            }
            var actionMember = eventHandler.GetType().GetMethod(actionName, new Type[] { typeof(IModify), typeof(IQuery) });
            if (actionMember == null)
            {
                return;
            }
            var handlerAction = Delegate.CreateDelegate(typeof(ModifyOperation), eventHandler, actionMember);
            if (handlerAction == null)
            {
                return;
            }
            var modifyEventHandler = new ModifyEventHandler()
            {
                EventType = EventType.ModifyExpression,
                HandlerRepositoryType = eventHandler.GetType(),
                ObjectType = objectType,
                Operation = (ModifyOperation)handlerAction
            };
            Subscribe(eventSource.GetType(), modifyEventHandler);
        }

        /// <summary>
        /// Subscribe condition remove
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeModify<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, ModifyOperation>> action)
        {
            SubscribeModify(typeof(TEventSource), typeof(TObject), typeof(TEventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        #endregion

        #region Query

        /// <summary>
        /// Subscribe query
        /// </summary>
        /// <param name="eventSourceType">Event source type</param>
        /// <param name="objectType">Object type</param>
        /// <param name="eventHandlerType">Event handler type</param>
        /// <param name="actionName">Action name</param>
        public static void SubscribeQuery(Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || string.IsNullOrWhiteSpace(actionName))
            {
                return;
            }
            var eventSource = ContainerManager.Resolve(eventSourceType);
            var eventHandler = ContainerManager.Resolve(eventHandlerType);
            if (eventSource == null || eventHandler == null)
            {
                return;
            }
            var parameterType = typeof(IEnumerable<>).MakeGenericType(objectType);
            var actionMember = eventHandler.GetType().GetMethod(actionName, new Type[] { parameterType });
            if (actionMember == null)
            {
                return;
            }
            var handlerAction = Delegate.CreateDelegate(typeof(QueryData<>).MakeGenericType(objectType), eventHandler, actionMember);
            if (handlerAction == null)
            {
                return;
            }
            var queryEventHandlerType = typeof(QueryEventHandler<>).MakeGenericType(objectType);
            var dataOperationEventHandler = (IRepositoryEventHandler)Activator.CreateInstance(queryEventHandlerType);
            var eventProperty = queryEventHandlerType.GetProperty("EventType");
            eventProperty.SetValue(dataOperationEventHandler, EventType.QueryData);
            var handlerRepositoryTypeProperty = queryEventHandlerType.GetProperty("HandlerRepositoryType");
            handlerRepositoryTypeProperty.SetValue(dataOperationEventHandler, eventHandler.GetType());
            var objectTypeProperty = queryEventHandlerType.GetProperty("ObjectType");
            objectTypeProperty.SetValue(dataOperationEventHandler, objectType);
            var operationProperty = queryEventHandlerType.GetProperty("Operation");
            operationProperty.SetValue(dataOperationEventHandler, handlerAction);

            Subscribe(eventSource.GetType(), dataOperationEventHandler);
        }

        /// <summary>
        /// Subscribe condition remove
        /// </summary>
        /// <typeparam name="TEventSource">Event source type</typeparam>
        /// <typeparam name="TEventHandler">Event handler type</typeparam>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="action">Action</param>
        public static void SubscribeQuery<TEventSource, TEventHandler, TObject>(Expression<Func<TEventHandler, QueryData<TObject>>> action)
        {
            SubscribeQuery(typeof(TEventSource), typeof(TObject), typeof(TEventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        #endregion

        #endregion

        #region Publish

        /// <summary>
        /// Gets event handlers
        /// </summary>
        /// <param name="eventSource">Event source type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="objectType">Object type</param>
        /// <returns></returns>
        static List<IRepositoryEventHandler> GetEventHandlers(Type eventSource, EventType eventType, Type objectType)
        {
            if (eventSource == null || objectType == null)
            {
                return new List<IRepositoryEventHandler>(0);
            }
            EventWarehouses.TryGetValue(eventSource.GUID, out Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>> sourceEvents);
            if (sourceEvents == null)
            {
                return new List<IRepositoryEventHandler>(0);
            }
            sourceEvents.TryGetValue(objectType.GUID, out Dictionary<EventType, List<IRepositoryEventHandler>> objectEvents);
            if (objectEvents == null)
            {
                return new List<IRepositoryEventHandler>(0);
            }
            objectEvents.TryGetValue(eventType, out List<IRepositoryEventHandler> eventHandlers);
            return eventHandlers ?? new List<IRepositoryEventHandler>(0);
        }

        /// <summary>
        /// Publish data operation
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishDataOperation<TObject>(Type eventSource, EventType eventType, IEnumerable<TObject> datas, ActivationOptions activationOptions, RepositoryEventCallback callback = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var eventHandlers = GetEventHandlers(eventSource, eventType, typeof(TObject));
            if (eventHandlers.IsNullOrEmpty())
            {
                callback?.Invoke(DataOperationEventExecuteResult.Empty);
                return;
            }
            foreach (var handler in eventHandlers)
            {
                DataEventHandler<TObject> eventHandler = handler as DataEventHandler<TObject>;
                if (eventHandler == null)
                {
                    continue;
                }
                eventHandler.Execute(datas, activationOptions);
            }
            callback?.Invoke(DataOperationEventExecuteResult.Empty);
        }

        /// <summary>
        /// Publish save event
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source</param>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishSave<TObject>(Type eventSource, IEnumerable<TObject> datas, ActivationOptions activationOptions = null, RepositoryEventCallback callback = null)
        {
            PublishDataOperation(eventSource, EventType.SaveObject, datas, activationOptions, callback);
        }

        /// <summary>
        /// Publish remove event
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source type</param>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishRemove<TObject>(Type eventSource, IEnumerable<TObject> datas, ActivationOptions activationOptions = null, RepositoryEventCallback callback = null)
        {
            PublishDataOperation(eventSource, EventType.RemoveObject, datas, activationOptions, callback);
        }

        /// <summary>
        /// Publish condition event
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="query">Query</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishCondition<TObject>(Type eventSource, EventType eventType, IQuery query, ActivationOptions activationOptions = null, RepositoryEventCallback callback = null)
        {
            var eventHandlers = GetEventHandlers(eventSource, eventType, typeof(TObject));
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    ConditionEventHandler eventHandler = handler as ConditionEventHandler;
                    if (eventHandler == null)
                    {
                        continue;
                    }
                    eventHandler.Execute(query, activationOptions);
                }
            }
            callback?.Invoke(DataOperationEventExecuteResult.Empty);
        }

        /// <summary>
        /// Publish remove event
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source type</param>
        /// <param name="query">Query</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishRemove<TObject>(Type eventSource, IQuery query, ActivationOptions activationOptions = null, RepositoryEventCallback callback = null)
        {
            PublishCondition<TObject>(eventSource, EventType.RemoveByCondition, query, activationOptions, callback);
        }

        /// <summary>
        /// Publish modify event
        /// </summary>
        /// <param name="eventSource">Event source type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="query">Query</param>
        /// <param name="activationOptions">Activation options</param>
        /// <param name="callback">Callback</param>
        public static void PublishModify<TObject>(Type eventSource, IModify modify, IQuery query, ActivationOptions activationOptions = null, RepositoryEventCallback callback = null)
        {
            var eventHandlers = GetEventHandlers(eventSource, EventType.ModifyExpression, typeof(TObject));
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    ModifyEventHandler eventHandler = handler as ModifyEventHandler;
                    if (eventHandler == null)
                    {
                        continue;
                    }
                    eventHandler.Execute(modify, query, activationOptions);
                }
            }
            callback?.Invoke(DataOperationEventExecuteResult.Empty);
        }

        /// <summary>
        /// Publis query event
        /// </summary>
        /// <typeparam name="TObject">Data type</typeparam>
        /// <param name="eventSource">Event source</param>
        /// <param name="datas">Datas</param>
        /// <param name="query">Query</param>
        /// <param name="callback">Callback</param>
        public static void PublishQuery<TObject>(Type eventSource, IEnumerable<TObject> datas, IQuery query, RepositoryEventCallback callback = null)
        {
            var eventHandlers = GetEventHandlers(eventSource, EventType.QueryData, typeof(TObject));
            var eventDatas = datas;
            if (!eventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in eventHandlers)
                {
                    QueryEventHandler<TObject> eventHandler = handler as QueryEventHandler<TObject>;
                    if (eventHandler == null)
                    {
                        continue;
                    }
                    eventDatas = eventHandler.Execute(eventDatas);
                }
            }
            callback?.Invoke(new QueryEventResult<TObject>()
            {
                Datas = eventDatas?.ToList() ?? new List<TObject>(0)
            });
        }

        #endregion
    }

    /// <summary>
    /// Event callback
    /// </summary>
    /// <param name="result">Repository event execute result</param>
    public delegate void RepositoryEventCallback(IRepositoryEventExecuteResult result);
}
