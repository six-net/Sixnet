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
using Sixnet.Development.Event;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data event bus
    /// </summary>
    public static partial class SixnetDataEventBus
    {
        #region Fields

        internal static readonly TimeEventManager timeDataEventManager = null;

        #endregion

        #region Constructor

        static SixnetDataEventBus()
        {
            timeDataEventManager = new TimeEventManager();
        }

        #endregion

        #region Publish

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        public static Task Publish(ISixnetDataEvent dataEvent, CancellationToken cancellationToken = default, Type entityType = null)
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
        public static Task Publish(IEnumerable<ISixnetDataEvent> dataEvents, CancellationToken cancellationToken = default, Type entityType = null)
        {
            return timeDataEventManager.Publish(dataEvents, cancellationToken, entityType);
        }

        /// <summary>
        /// Publish starting data event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        internal static Task PublishStartingDataEvent(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, bool async, CancellationToken cancellationToken)
        {
            var dataEvents = new List<ISixnetDataEvent>();
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
        internal static Task PublishExecutedDataEvent(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, CancellationToken cancellationToken)
        {
            var dataEvents = new List<ISixnetDataEvent>();
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
        internal static Task PublishQueriedEvent<TData>(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, IEnumerable<TData> datas, CancellationToken cancellationToken)
        {
            return Publish(QueriedDataEvent<TData>.Create(dataClient, dataCommand, datas), cancellationToken);
        }

        /// <summary>
        /// Publish got value event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="value">Value</param>
        internal static Task PublishGotValueEvent(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, dynamic value, CancellationToken cancellationToken)
        {
            return Publish(GotValueEvent.Create(dataClient, dataCommand, value), cancellationToken);
        }

        /// <summary>
        /// Publish checked event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="hasValue">Has value</param>
        internal static Task PublishCheckedEvent(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, bool hasValue, CancellationToken cancellationToken)
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
        public static void SubscribeAll(ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Action<ISixnetDataEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDataEvent e, CancellationToken ct)
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
        public static void SubscribeAll(Func<ISixnetDataEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDataEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll(Func<ISixnetDataEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(new DefaultDataEventHandler<ISixnetDataEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetDataEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetDataEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetDataEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void Subscribe(Type eventType, ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.Subscribe(eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void Subscribe<TEvent>(ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe<TEvent>(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent>(ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null, bool applyToEntity = false, params Type[] entityTypes) where TEvent : class, ISixnetDataEvent
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
        public static void SubscribeEntityAll(Type entityType, ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll(entityType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntityAll<TEntity>(ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            SubscribeEntityAll(typeof(TEntity), handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntityAll<TEntity>(Action<ISixnetDataEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDataEvent e, CancellationToken ct)
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
        public static void SubscribeEntityAll<TEntity>(Func<ISixnetDataEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDataEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeEntityAll<TEntity>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity>(Func<ISixnetDataEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            SubscribeEntityAll(typeof(TEntity), new DefaultDataEventHandler<ISixnetDataEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Action<ISixnetDataEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Func<ISixnetDataEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe entity all data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntityAll<TEntity, THandler>(Expression<Func<THandler, Func<ISixnetDataEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDataEventManager.SubscribeModelAll<TEntity>(GetDataEventHandlerByExecutorName<ISixnetDataEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Entity specific event

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity(Type entityType, Type eventType, ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModel(entityType, eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="entityType">Entity type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity<TEvent>(Type entityType, ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            SubscribeEntity(entityType, typeof(TEvent), handler, configure);
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(ISixnetDataEventHandler handler, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            SubscribeEntity<TEvent>(typeof(TEntity), handler, configure);
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntity<TEntity, TEvent>(Action<TEvent> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(Func<TEvent, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeEntity<TEntity, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent, THandler>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
        {
            timeDataEventManager.SubscribeModel<TEntity, TEvent, THandler>(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe entity data event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeEntity<TEntity, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DataEventHandlerOptions> configure = null) where TEvent : class, ISixnetDataEvent
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
            var allEntityConfigs = SixnetEntityManager.GetAllEntityConfigs();
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
                            var relationEntityConfig = SixnetEntityManager.GetEntityConfig(relationEntityTypeGuid);

                            var deletingHandler = Activator.CreateInstance(typeof(DefaultCascadingDeletingEventHandler<>).MakeGenericType(entityConfig.EntityType)) as ISixnetDataEventHandler;
                            SubscribeEntity(relationEntityConfig.EntityType, typeof(CascadingDeletingDataEvent), deletingHandler);

                            var asyncDeletingHandler = Activator.CreateInstance(typeof(DefaultCascadingDeletingAsyncEventHandler<>).MakeGenericType(entityConfig.EntityType)) as ISixnetDataEventHandler;
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
        internal static Task HandleWorkCompleted(IEnumerable<ISixnetDataEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            return timeDataEventManager.HandleWorkCompleted(eventDatas, cancellationToken);
        }

        #endregion

        #region Util

        static Action<TimeEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(DataEventHandlerOptions options)
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

        static Action<TimeEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(Action<DataEventHandlerOptions> configure)
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
        static ISixnetDataEventHandler GetDataEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, DataEventHandlerOptions options = null) where TEvent : ISixnetDataEvent
        {
            var handlerExecutor = EventManager.GetEventHandlerExecutor<TEvent>(handlerType, handlerExecutorName);
            return new DefaultDataEventHandler<TEvent>(handlerExecutor, options);
        }

        #endregion
    }
}
