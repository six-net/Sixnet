using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Data access event bus
    /// </summary>
    public static class DataAccessEventBus
    {
        internal static readonly DataAccessEventManager GlobalDataAccessEventManager = null;

        static DataAccessEventBus()
        {
            GlobalDataAccessEventManager = new DataAccessEventManager();
        }

        #region Publish

        #region Publish data access event

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataAccessEvents">Data access event</param>
        public static void Publish(params IDataAccessEvent[] dataAccessEvents)
        {
            if (dataAccessEvents.IsNullOrEmpty())
            {
                return;
            }
            foreach (var dataAccessEvent in dataAccessEvents)
            {
                switch (dataAccessEvent.EventType)
                {
                    case DataAccessEventType.AggregateFunction:
                    case DataAccessEventType.QueryData:
                    case DataAccessEventType.CheckData:
                        GlobalDataAccessEventManager.Publish(dataAccessEvent);
                        break;
                    default:
                        WorkManager.Current?.PublishDataAccessEvent(dataAccessEvent);
                        break;
                }
            }
        }

        #endregion

        #region Publish add event

        /// <summary>
        /// Publish add event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="values">Values</param>
        /// <returns></returns>
        public static void PublishAddEvent<T>(T data, Dictionary<string, dynamic> values)
        {
            if (data == null && values.IsNullOrEmpty())
            {
                return;
            }
            var addEvent = new AddDataEvent()
            {
                EntityType = typeof(T),
                Data = data,
                Values = values
            };
            Publish(addEvent);
        }

        #endregion

        #region Publish modify data event

        /// <summary>
        /// Publish modify data event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="originalValues">Original values</param>
        /// <param name="newValues">New values</param>
        /// <param name="query">Query</param>
        /// <returns></returns>
        public static void PublishModifyDataEvent<T>(Dictionary<string, dynamic> originalValues, Dictionary<string, dynamic> newValues, IQuery query)
        {
            if (originalValues.IsNullOrEmpty() && newValues.IsNullOrEmpty())
            {
                return;
            }
            var updateEvent = new ModifyDataEvent()
            {
                EntityType = typeof(T),
                OriginalValues = originalValues,
                NewValues = newValues,
                Query = query
            };
            Publish(updateEvent);
        }

        #endregion

        #region Publish modifyexpression event

        /// <summary>
        /// Publish modifyexpression event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="modifyValues">Modify values</param>
        /// <param name="query">Query</param>
        /// <returns></returns>
        public static void PublishModifyExpressionEvent<T>(Dictionary<string, IModifyValue> modifyValues, IQuery query)
        {
            if (modifyValues.IsNullOrEmpty())
            {
                return;
            }
            var modifyEvent = new ModifyExpressionEvent()
            {
                EntityType = typeof(T),
                ModifyValues = modifyValues,
                Query = query
            };
            Publish(modifyEvent);
        }

        #endregion

        #region Publish delete event

        /// <summary>
        /// Publish delete data event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="keyValues">Key values</param>
        /// <returns></returns>
        public static void PublishDeleteEvent<T>(T data, Dictionary<string, dynamic> keyValues)
        {
            var deleteEvent = new DeleteDataEvent()
            {
                EntityType = typeof(T),
                Data = data,
                KeyValues = keyValues
            };
            Publish(deleteEvent);
        }

        #endregion

        #region Publish Delete by condition event

        /// <summary>
        /// Publish Delete by condition event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query</param>
        public static void PublishDeleteByCondition<T>(IQuery query)
        {
            var deleteEvent = new DeleteByConditionEvent()
            {
                EntityType = typeof(T),
                Query = query
            };
            Publish(deleteEvent);
        }

        #endregion

        #region Publish query event

        /// <summary>
        /// Publish query event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query</param>
        /// <param name="datas">Datas</param>
        /// <returns></returns>
        public static void PublishQueryEvent<T>(IQuery query, IEnumerable<T> datas)
        {
            var queryEvent = new QueryDataEvent()
            {
                EntityType = typeof(T),
                Query = query,
                Datas = datas
            };
            Publish(queryEvent);
        }

        #endregion

        #region Publish check data event

        /// <summary>
        /// Publish check data event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query</param>
        /// <param name="hasValue">Whether has value</param>
        public static void PublishCheckDataEvent<T>(IQuery query, bool hasValue)
        {
            var checkEvent = new CheckDataEvent()
            {
                EntityType = typeof(T),
                Query = query,
                HasValue = hasValue
            };
            Publish(checkEvent);
        }

        #endregion

        #region Publish aggregate function event

        /// <summary>
        /// Publish aggregate function event
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="operateType">Operate type</param>
        /// <param name="value">Value</param>
        /// <param name="query">Query</param>
        /// <returns></returns>
        public static void PublishAggregateFunctionEvent<T>(OperateType operateType, dynamic value, IQuery query)
        {
            var funcEvent = new AggregateFunctionEvent()
            {
                EntityType = typeof(T),
                OperateType = operateType,
                Query = query,
                Value = value
            };
            Publish(funcEvent);
        }

        #endregion

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventhandler">Event handler</param>
        public static void SubscribeAll(IDataAccessEventHandler eventhandler)
        {
            GlobalDataAccessEventManager.SubscribeAll(eventhandler);
        }

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public static void SubscribeAll(Func<IDataAccessEvent, DataAccessEventExecuteResult> eventHandleOperation)
        {
            GlobalDataAccessEventManager.SubscribeAll(eventHandleOperation);
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="eventHandler">Event handler</param>
        public static void Subscribe(Type eventType, IDataAccessEventHandler eventHandler)
        {
            GlobalDataAccessEventManager.Subscribe(eventType, eventHandler);
        }

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public static void Subscribe<TEvent>(IDataAccessEventHandler eventHandler) where TEvent : class, IDataAccessEvent
        {
            Subscribe(typeof(TEvent), eventHandler);
        }

        /// <summary>
        /// Subscribe data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public static void Subscribe<TEvent>(Func<TEvent, DataAccessEventExecuteResult> eventHandleOperation) where TEvent : class, IDataAccessEvent
        {
            GlobalDataAccessEventManager.Subscribe(eventHandleOperation);
        }

        #endregion

        #region Entity overall

        /// <summary>
        /// Subscribe entity data access event in global area
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventHandler">Event handler</param>
        public static void SubscribeEntityAll(Type entityType, IDataAccessEventHandler eventHandler)
        {
            GlobalDataAccessEventManager.SubscribeEntityAll(entityType, eventHandler);
        }

        /// <summary>
        /// Subscribe entity data access event in global area
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        public static void SubscribeEntityAll<T>(IDataAccessEventHandler eventHandler)
        {
            SubscribeEntityAll(typeof(T), eventHandler);
        }

        /// <summary>
        /// Subscribe data access event in global area
        /// </summary>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public static void SubscribeEntityAll<T>(Func<IDataAccessEvent, DataAccessEventExecuteResult> eventHandleOperation)
        {
            GlobalDataAccessEventManager.SubscribeEntityAll<T>(eventHandleOperation);
        }

        #endregion

        #region Entity specific event

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity(Type entityType, Type eventType, IDataAccessEventHandler handler)
        {
            GlobalDataAccessEventManager.SubscribeEntity(entityType, eventType, handler);
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventHandler">Event handler</param>
        public static void SubscribeEntity<TEvent>(Type entityType, IDataAccessEventHandler eventHandler) where TEvent : class, IDataAccessEvent
        {
            SubscribeEntity(entityType, typeof(TEvent), eventHandler);
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandler">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(IDataAccessEventHandler eventHandler) where TEvent : class, IDataAccessEvent
        {
            SubscribeEntity<TEvent>(typeof(TEntity), eventHandler);
        }

        /// <summary>
        /// Subscribe entity data access event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventHandleOperation">Event handle operation</param>
        public static void SubscribeEntity<TEntity, TEvent>(Func<TEvent, DataAccessEventExecuteResult> eventHandleOperation) where TEvent : class, IDataAccessEvent
        {
            GlobalDataAccessEventManager.SubscribeEntity<TEntity, TEvent>(eventHandleOperation);
        }

        #endregion

        #region Add event

        /// <summary>
        /// Subscribe add event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeAdd(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<AddDataEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<AddDataEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe add event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeAdd(Func<AddDataEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<AddDataEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<AddDataEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Update event

        /// <summary>
        /// Subscribe update event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeUpdate(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<ModifyDataEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<ModifyDataEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe update event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeUpdate(Func<ModifyDataEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<ModifyDataEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<ModifyDataEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Modifyexpression event

        /// <summary>
        /// Subscribe modify expression event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeModifyExpression(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<ModifyExpressionEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<ModifyExpressionEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe modify expression event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeModifyExpression(Func<ModifyExpressionEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<ModifyExpressionEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<ModifyExpressionEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Delete event

        /// <summary>
        /// Subscribe delete event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeDelete(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<DeleteDataEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<DeleteDataEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe delete event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeDelete(Func<DeleteDataEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<DeleteDataEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<DeleteDataEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Delete by condition event

        /// <summary>
        /// Subscribe Delete by condition event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeDeleteByCondition(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<DeleteByConditionEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<DeleteByConditionEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe Delete by condition event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeDeleteByCondition(Func<DeleteByConditionEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<DeleteByConditionEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<DeleteByConditionEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Query event

        /// <summary>
        /// Subscribe query event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeQuery(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<QueryDataEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<QueryDataEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe query event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeQuery(Func<QueryDataEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<QueryDataEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<QueryDataEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Check data event

        /// <summary>
        /// Subscribe check data event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeCheckData(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<CheckDataEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<CheckDataEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe check data event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeCheckData(Func<CheckDataEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<CheckDataEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<CheckDataEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #region Aggregate function event

        /// <summary>
        /// Subscribe aggregate function event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeAggregateFunction(IDataAccessEventHandler eventHandler, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandler == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<AggregateFunctionEvent>(entity, eventHandler);
                }
            }
            else
            {
                Subscribe<AggregateFunctionEvent>(eventHandler);
            }
        }

        /// <summary>
        /// Subscribe aggregate function event
        /// </summary>
        /// <param name="eventHandleOperation">Event operation</param>
        /// <param name="applyToEntity">Apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void SubscribeAggregateFunction(Func<AggregateFunctionEvent, DataAccessEventExecuteResult> eventHandleOperation, bool applyToEntity = false, params Type[] entityTypes)
        {
            if (eventHandleOperation == null || (applyToEntity && entityTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToEntity)
            {
                var handler = new DefaultDataAccessEventHandler<AggregateFunctionEvent>()
                {
                    ExecuteEventOperation = eventHandleOperation
                };
                foreach (var entity in entityTypes)
                {
                    SubscribeEntity<AggregateFunctionEvent>(entity, handler);
                }
            }
            else
            {
                Subscribe(eventHandleOperation);
            }
        }

        #endregion

        #endregion

        #region Trigger event

        /// <summary>
        /// Trigger data access event
        /// </summary>
        /// <param name="dataAccessEvents">Data access events</param>
        /// <returns></returns>
        internal static void TriggerDataAccessEvent(IEnumerable<IDataAccessEvent> dataAccessEvents)
        {
            GlobalDataAccessEventManager.Publish(dataAccessEvents);
        }

        #endregion
    }
}
