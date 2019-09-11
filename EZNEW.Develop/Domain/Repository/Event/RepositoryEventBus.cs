using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Framework.Extension;
using EZNEW.Framework.IoC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// repository event bus
    /// </summary>
    public static class RepositoryEventBus
    {
        /// <summary>
        /// key:source repository->operation data->event type
        /// </summary>
        static Dictionary<Guid, Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>> eventWarehouse = new Dictionary<Guid, Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>>();

        #region Subscribe

        /// <summary>
        /// subscribe event
        /// </summary>
        /// <param name="repositoryEventHandler">event handler</param>
        public static void Subscribe(Type eventSource, IRepositoryEventHandler repositoryEventHandler)
        {
            if (eventSource == null || repositoryEventHandler == null || repositoryEventHandler.ObjectType == null || repositoryEventHandler.HandlerRepositoryType == null)
            {
                return;
            }
            var eventSourceId = eventSource.GUID;
            var objectId = repositoryEventHandler.ObjectType.GUID;
            if (!eventWarehouse.TryGetValue(eventSourceId, out var sourceEvents) || sourceEvents.IsNullOrEmpty())
            {
                sourceEvents = new Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>>();
                eventWarehouse[eventSourceId] = sourceEvents;
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

        #region data operation

        /// <summary>
        /// subscribe data operation
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventSourceType"></param>
        /// <param name="objectType"></param>
        /// <param name="eventHandlerType"></param>
        /// <param name="actionName"></param>
        public static void SubscribeDataOperation(EventType eventType, Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || actionName.IsNullOrEmpty())
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

        public static void SubscribeDataOperation<EventSource, EventHandler, T>(EventType eventType, Expression<Func<EventHandler, DataOperation<T>>> action)
        {
            SubscribeDataOperation(eventType, typeof(EventSource), typeof(T), typeof(EventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        /// <summary>
        /// subscribe save
        /// </summary>
        /// <typeparam name="EventSource">event source</typeparam>
        /// <typeparam name="EventHandler">event handler</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action</param>
        public static void SubscribeSave<EventSource, EventHandler, T>(Expression<Func<EventHandler, DataOperation<T>>> action)
        {
            SubscribeDataOperation<EventSource, EventHandler, T>(EventType.SaveObject, action);
        }

        /// <summary>
        /// subscribe object remove
        /// </summary>
        /// <typeparam name="EventSource">event source</typeparam>
        /// <typeparam name="EventHandler">event handler</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action</param>
        public static void SubscribeObjectRemove<EventSource, EventHandler, T>(Expression<Func<EventHandler, DataOperation<T>>> action)
        {
            SubscribeDataOperation<EventSource, EventHandler, T>(EventType.RemoveObject, action);
        }

        /// <summary>
        /// subscribe remove(both object and condition)
        /// </summary>
        /// <typeparam name="EventSource">event source</typeparam>
        /// <typeparam name="EventHandler">event handler</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action</param>
        public static void SubscribeRemove<EventSource, EventHandler, T>(Expression<Func<EventHandler, DataOperation<T>>> objectRemoveAction = null, Expression<Func<EventHandler, ConditionOperation>> conditionRemoveAction = null)
        {
            //object remove
            if (objectRemoveAction != null)
            {
                SubscribeObjectRemove<EventSource, EventHandler, T>(objectRemoveAction);
            }
            //condition remove
            if (conditionRemoveAction != null)
            {
                SubscribeConditionRemove<EventSource, EventHandler, T>(conditionRemoveAction);
            }
        }

        #endregion

        #region condition operation

        /// <summary>
        /// subscribe condition remove
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventSourceType"></param>
        /// <param name="objectType"></param>
        /// <param name="eventHandlerType"></param>
        /// <param name="actionName"></param>
        public static void SubscribeConditionOperation(EventType eventType, Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || actionName.IsNullOrEmpty())
            {
                return;
            }
            var eventSource = ContainerManager.Resolve(eventSourceType);
            var eventHandler = ContainerManager.Resolve(eventHandlerType);
            if (eventSource == null || eventHandler == null)
            {
                return;
            }
            var actionMember = eventHandler.GetType().GetMethod(actionName, new Type[] { typeof(IQuery) });
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
        /// subscribe condition remove
        /// </summary>
        /// <typeparam name="EventSource"></typeparam>
        /// <typeparam name="EventHandler"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        /// <param name="query"></param>
        public static void SubscribeConditionOperation<EventSource, EventHandler, T>(EventType eventType, Expression<Func<EventHandler, ConditionOperation>> action)
        {
            SubscribeConditionOperation(eventType, typeof(EventSource), typeof(T), typeof(EventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        /// <summary>
        /// subscribe save
        /// </summary>
        /// <typeparam name="EventSource">event source</typeparam>
        /// <typeparam name="EventHandler">event handler</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action</param>
        public static void SubscribeConditionRemove<EventSource, EventHandler, T>(Expression<Func<EventHandler, ConditionOperation>> action)
        {
            SubscribeConditionOperation<EventSource, EventHandler, T>(EventType.RemoveByCondition, action);
        }

        #endregion

        #region modify

        /// <summary>
        /// subscribe condition remove
        /// </summary>
        /// <param name="eventSourceType"></param>
        /// <param name="objectType"></param>
        /// <param name="eventHandlerType"></param>
        /// <param name="actionName"></param>
        public static void SubscribeModify(Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || actionName.IsNullOrEmpty())
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
        /// subscribe condition remove
        /// </summary>
        /// <typeparam name="EventSource"></typeparam>
        /// <typeparam name="EventHandler"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="query"></param>
        public static void SubscribeModify<EventSource, EventHandler, T>(Expression<Func<EventHandler, ModifyOperation>> action)
        {
            SubscribeModify(typeof(EventSource), typeof(T), typeof(EventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        #endregion

        #region query

        /// <summary>
        /// subscribe query
        /// </summary>
        /// <param name="eventSourceType"></param>
        /// <param name="objectType"></param>
        /// <param name="eventHandlerType"></param>
        /// <param name="actionName"></param>
        public static void SubscribeQuery(Type eventSourceType, Type objectType, Type eventHandlerType, string actionName)
        {
            if (eventSourceType == null || eventHandlerType == null || objectType == null || actionName.IsNullOrEmpty())
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
        /// subscribe condition remove
        /// </summary>
        /// <typeparam name="EventSource"></typeparam>
        /// <typeparam name="EventHandler"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="query"></param>
        public static void SubscribeQuery<EventSource, EventHandler, T>(Expression<Func<EventHandler, QueryData<T>>> action)
        {
            SubscribeQuery(typeof(EventSource), typeof(T), typeof(EventHandler), ExpressionHelper.GetExpressionPropertyName(ExpressionHelper.GetLastChildExpression(action)));
        }

        #endregion

        #endregion

        #region Publish

        /// <summary>
        /// get event handlers
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventType">event type</param>
        /// <param name="objectType">object type</param>
        /// <returns></returns>
        static List<IRepositoryEventHandler> GetEventHandlers(Type eventSource, EventType eventType, Type objectType)
        {
            if (eventSource == null || objectType == null)
            {
                return new List<IRepositoryEventHandler>(0);
            }
            eventWarehouse.TryGetValue(eventSource.GUID, out Dictionary<Guid, Dictionary<EventType, List<IRepositoryEventHandler>>> sourceEvents);
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
        /// publish data operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventSource">event source</param>
        /// <param name="eventType">event type</param>
        /// <param name="datas">datas</param>
        /// <param name="callback">callback</param>
        public static void PublishDataOperation<T>(Type eventSource, EventType eventType, IEnumerable<T> datas, RepositoryEventCallback callback = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var eventHandlers = GetEventHandlers(eventSource, eventType, typeof(T));
            if (eventHandlers.IsNullOrEmpty())
            {
                callback?.Invoke(DataOperationEventResult.Empty);
                return;
            }
            foreach (var handler in eventHandlers)
            {
                DataEventHandler<T> eventHandler = handler as DataEventHandler<T>;
                if (eventHandler == null)
                {
                    continue;
                }
                eventHandler.Execute(datas);
            }
            callback?.Invoke(DataOperationEventResult.Empty);
        }

        /// <summary>
        /// publish save event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventSource">event source</param>
        /// <param name="datas">datas</param>
        /// <param name="callback">callback</param>
        public static void PublishSave<T>(Type eventSource, IEnumerable<T> datas, RepositoryEventCallback callback = null)
        {
            PublishDataOperation(eventSource, EventType.SaveObject, datas, callback);
        }

        /// <summary>
        /// publish remove event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventSource">event source</param>
        /// <param name="datas">datas</param>
        /// <param name="callback">callback</param>
        public static void PublishRemove<T>(Type eventSource, IEnumerable<T> datas, RepositoryEventCallback callback = null)
        {
            PublishDataOperation(eventSource, EventType.RemoveObject, datas, callback);
        }

        /// <summary>
        /// publish condition
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventType">event type</param>
        /// <param name="query">query</param>
        /// <param name="callback">callback</param>
        public static void PublishCondition<T>(Type eventSource, EventType eventType, IQuery query, RepositoryEventCallback callback = null)
        {
            var eventHandlers = GetEventHandlers(eventSource, eventType, typeof(T));
            if (eventHandlers.IsNullOrEmpty())
            {
                callback?.Invoke(DataOperationEventResult.Empty);
                return;
            }
            foreach (var handler in eventHandlers)
            {
                ConditionEventHandler eventHandler = handler as ConditionEventHandler;
                if (eventHandler == null)
                {
                    continue;
                }
                eventHandler.Execute(query);
            }
            callback?.Invoke(DataOperationEventResult.Empty);
        }

        /// <summary>
        /// publish remove
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventSource">event source</param>
        /// <param name="query">query</param>
        /// <param name="callback">callback</param>
        public static void PublishRemove<T>(Type eventSource, IQuery query, RepositoryEventCallback callback = null)
        {
            PublishCondition<T>(eventSource, EventType.RemoveByCondition, query, callback);
        }

        /// <summary>
        /// publish modify
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventType">event type</param>
        /// <param name="query">query</param>
        /// <param name="callback">callback</param>
        public static void PublishModify<T>(Type eventSource, IModify modify, IQuery query, RepositoryEventCallback callback = null)
        {
            if (modify == null)
            {
                return;
            }
            var eventHandlers = GetEventHandlers(eventSource, EventType.ModifyExpression, typeof(T));
            if (eventHandlers.IsNullOrEmpty())
            {
                callback?.Invoke(DataOperationEventResult.Empty);
                return;
            }
            foreach (var handler in eventHandlers)
            {
                ModifyEventHandler eventHandler = handler as ModifyEventHandler;
                if (eventHandler == null)
                {
                    continue;
                }
                eventHandler.Execute(modify, query);
            }
            callback?.Invoke(DataOperationEventResult.Empty);
        }

        /// <summary>
        /// publis query event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventSource">event source</param>
        /// <param name="datas">datas</param>
        /// <param name="callback">callback</param>
        public static void PublishQuery<T>(Type eventSource, IEnumerable<T> datas, RepositoryEventCallback callback = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var eventHandlers = GetEventHandlers(eventSource, EventType.QueryData, typeof(T));
            if (eventHandlers.IsNullOrEmpty())
            {
                callback?.Invoke(new QueryEventResult<T>()
                {
                    Datas = datas.ToList()
                });
                return;
            }
            var eventDatas = datas;
            foreach (var handler in eventHandlers)
            {
                QueryEventHandler<T> eventHandler = handler as QueryEventHandler<T>;
                if (eventHandler == null)
                {
                    continue;
                }
                eventDatas = eventHandler.Execute(eventDatas);
            }
            callback?.Invoke(new QueryEventResult<T>()
            {
                Datas = eventDatas.ToList()
            });
        }

        #endregion
    }
}
