using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Entity;
using Sixnet.Development.Events;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data event bus
    /// </summary>
    public static partial class DataEventBus
    {
        #region Fields

        internal static readonly TimeSixnetEventManager timeDataEventManager = null;

        #endregion

        #region Constructor

        static DataEventBus()
        {
            timeDataEventManager = new TimeSixnetEventManager();
        }

        #endregion

        #region Publish

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        public static Task Publish(IDataEvent dataEvent, CancellationToken cancellationToken = default, Type entityType = null)
        {
            return timeDataEventManager.Publish(dataEvent, cancellationToken, entityType);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static Task Publish(IEnumerable<IDataEvent> dataEvents, CancellationToken cancellationToken = default, Type entityType = null)
        {
            return timeDataEventManager.Publish(dataEvents, cancellationToken, entityType);
        }

        /// <summary>
        /// Publish starting data event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        internal static Task PublishStartingDataEvent(IDataClient dataClient, DataCommand dataCommand, bool async, CancellationToken cancellationToken)
        {
            var dataEvents = new List<IDataEvent>();
            if (dataClient != null && dataCommand != null)
            {
                switch (dataCommand.OperationType)
                {
                    case DataOperationType.Query:
                        dataEvents.Add(QueryingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Insert:
                        dataEvents.Add(AddingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Update:
                        dataEvents.Add(UpdatingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Delete:
                        dataEvents.Add(DeletingDataEvent.Create(dataClient, dataCommand));
                        if (async)
                        {
                            dataEvents.Add(CascadingDeletingAsyncDataEvent.Create(dataClient, dataCommand));
                        }
                        else
                        {
                            dataEvents.Add(CascadingDeletingDataEvent.Create(dataClient, dataCommand));
                        }
                        break;
                    case DataOperationType.Exist:
                        dataEvents.Add(CheckingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Scalar:
                        dataEvents.Add(GettingValueEvent.Create(dataClient, dataCommand));
                        break;
                }
            }
            return Publish(dataEvents, cancellationToken, dataCommand.GetEntityType());
        }

        /// <summary>
        /// Publish executed data event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        internal static Task PublishExecutedDataEvent(IDataClient dataClient, DataCommand dataCommand, CancellationToken cancellationToken)
        {
            var dataEvents = new List<IDataEvent>();
            if (dataClient != null && dataCommand != null)
            {
                switch (dataCommand.OperationType)
                {
                    case DataOperationType.Insert:
                        dataEvents.Add(AddedDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Update:
                        dataEvents.Add(UpdatedDataEvent.Create(dataClient, dataCommand));
                        break;
                    case DataOperationType.Delete:
                        dataEvents.Add(DeletedDataEvent.Create(dataClient, dataCommand));
                        break;
                }
            }
            return Publish(dataEvents, cancellationToken, dataCommand.GetEntityType());
        }

        /// <summary>
        /// Publish queried event
        /// </summary>
        /// <typeparam name="TData">Data type</typeparam>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="datas">Datas</param>
        internal static Task PublishQueriedEvent<TData>(IDataClient dataClient, DataCommand dataCommand, IEnumerable<TData> datas, CancellationToken cancellationToken)
        {
            return Publish(QueriedDataEvent<TData>.Create(dataClient, dataCommand, datas), cancellationToken);
        }

        /// <summary>
        /// Publish got value event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="value">Value</param>
        internal static Task PublishGotValueEvent(IDataClient dataClient, DataCommand dataCommand, dynamic value, CancellationToken cancellationToken)
        {
            return Publish(GotValueEvent.Create(dataClient, dataCommand, value), cancellationToken);
        }

        /// <summary>
        /// Publish checked event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="hasValue">Has value</param>
        internal static Task PublishCheckedEvent(IDataClient dataClient, DataCommand dataCommand, bool hasValue, CancellationToken cancellationToken)
        {
            return Publish(CheckedDataEvent.Create(dataClient, dataCommand, hasValue), cancellationToken);
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public static void SubscribeAll(IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Action<IDataEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDataEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Func<IDataEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDataEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll(Func<IDataEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(new DefaultDataEventHandler<IDataEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Action<IDataEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<IDataEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<IDataEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void Subscribe(Type eventType, IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.Subscribe(eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void Subscribe<TEvent>(IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe<TEvent>(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handler">Event handler</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent>(IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe<TEvent>(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        /// <summary>
        /// Subscibe data event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToEntity">Whether apply to entity</param>
        /// <param name="entityTypes">Entity types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, IDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure), applyToEntity, entityTypes);
        }

        #endregion

        #region Entity overall

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntityAll(Type entityType, IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll(entityType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntityAll<TEntity>(IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            SubscribeEntityAll(typeof(TEntity), handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntityAll<TEntity>(Action<IDataEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDataEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeEntityAll<TEntity>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity>(Func<IDataEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDataEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeEntityAll<TEntity>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity>(Func<IDataEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            SubscribeEntityAll(typeof(TEntity), new DefaultDataEventHandler<IDataEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Action<IDataEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Func<IDataEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Func<IDataEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<IDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Entity specific event

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity(Type entityType, Type eventType, IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModel(entityType, eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="entityType">Entity type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity<TEvent>(Type entityType, IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            SubscribeEntity(entityType, typeof(TEvent), handler, configure);
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(IDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            SubscribeEntity<TEvent>(typeof(TEntity), handler, configure);
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntity<TEntity, TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent, THandler>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent, THandler>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, IDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent, THandler>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        #endregion

        #region Subscribe default event

        /// <summary>
        /// Subscribe default data evnet
        /// </summary>
        internal static void SubscribeDefaultDataEvent()
        {
            var allEntityConfigs = EntityManager.GetAllEntityConfigurations();
            if (!allEntityConfigs.IsNullOrEmpty())
            {
                foreach (var entityConfig in allEntityConfigs)
                {
                    if (entityConfig == null || entityConfig.RelationFields.IsNullOrEmpty())
                    {
                        continue;
                    }
                    foreach (var relationItem in entityConfig.RelationFields)
                    {
                        //Deleting data event
                        if (relationItem.Value.Any(c => (c.Value.Behavior & RelationBehavior.CascadingDelete) == RelationBehavior.CascadingDelete))
                        {
                            var relationEntityTypeGuid = relationItem.Key;
                            var relationEntityConfig = EntityManager.GetEntityConfiguration(relationEntityTypeGuid);

                            var deletingHandler = Activator.CreateInstance(typeof(CascadingDeletingEventHandler<>).MakeGenericType(entityConfig.EntityType)) as IDataEventHandler;
                            SubscribeEntity(relationEntityConfig.EntityType, typeof(CascadingDeletingDataEvent), deletingHandler);

                            var asyncDeletingHandler = Activator.CreateInstance(typeof(CascadingDeletingAsyncEventHandler<>).MakeGenericType(entityConfig.EntityType)) as IDataEventHandler;
                            SubscribeEntity(relationEntityConfig.EntityType, typeof(CascadingDeletingAsyncDataEvent), asyncDeletingHandler);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Handle

        /// <summary>
        /// Handle work completed handler
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <returns></returns>
        internal static Task HandleWorkCompleted(IEnumerable<IDataEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            return timeDataEventManager.HandleWorkCompleted(eventDatas, cancellationToken);
        }

        #endregion

        #region Util

        static Action<TimeSixnetEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(DataEventHandlerOptions options)
        {
            return o =>
            {
                if (options != null)
                {
                    o.Async = options.Async;
                    o.TriggerTime = options.TriggerTime;
                }
            };
        }

        static Action<TimeSixnetEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(Action<DataEventHandlerOptions> configure)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            return o =>
            {
                o.Async = options.Async;
                o.TriggerTime = options.TriggerTime;
            };
        }

        static DataEventHandlerOptions GetConfigurationDataEventHandlerOptions(Action<DataEventHandlerOptions> configure)
        {
            var options = new DataEventHandlerOptions();
            configure?.Invoke(options);
            return options;
        }

        /// <summary>
        /// Get event handler by executor name
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <returns></returns>
        static IDataEventHandler GetDataEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, DataEventHandlerOptions options = null) where TEvent : IDataEvent
        {
            var handlerExecutor = SixnetEventManager.GetEventHandlerExecutor<TEvent>(handlerType, handlerExecutorName);
            return new DefaultDataEventHandler<TEvent>(handlerExecutor, options);
        }

        #endregion
    }
}
