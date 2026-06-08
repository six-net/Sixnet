// "Company © 2025. All rights reserved."

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Entity;
using Sixnet.Development.Events.Data;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Sixnet event bus
    /// </summary>
    public static class SixnetEventBus
    {
        #region Fields

        internal static readonly SixnetTimeEventManager timeDataEventManager = null;

        #endregion

        #region Constructor

        static SixnetEventBus()
        {
            timeDataEventManager = new SixnetTimeEventManager();
        }

        #endregion

        #region Publish

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        public static Task PublishAsync(ISixnetEvent dataEvent, CancellationToken cancellationToken = default, Type modelType = null)
        {
            return timeDataEventManager.PublishAsync(dataEvent, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public static Task PublishAsync(IEnumerable<ISixnetEvent> dataEvents, CancellationToken cancellationToken = default, Type modelType = null)
        {
            return timeDataEventManager.PublishAsync(dataEvents, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish data event
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        public static void Publish(ISixnetEvent dataEvent, CancellationToken cancellationToken = default, Type modelType = null)
        {
            timeDataEventManager.PublishAsync(dataEvent, cancellationToken, modelType).Wait();
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="dataEvents">Data events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public static void Publish(IEnumerable<ISixnetEvent> dataEvents, CancellationToken cancellationToken = default, Type modelType = null)
        {
            timeDataEventManager.PublishAsync(dataEvents, cancellationToken, modelType).Wait();
        }

        /// <summary>
        /// Publish starting data event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        internal static Task PublishStartingDataEventAsync(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, bool async, CancellationToken cancellationToken)
        {
            var dataEvents = new List<ISixnetEvent>();
            if (dataClient != null && dataCommand != null)
            {
                switch (dataCommand.OperationType)
                {
                    case SixnetDataOperationType.Query:
                        dataEvents.Add(SixnetQueryingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Insert:
                        dataEvents.Add(SixnetAddingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Update:
                        dataEvents.Add(SixnetUpdatingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Delete:
                        dataEvents.Add(SixnetDeletingDataEvent.Create(dataClient, dataCommand));
                        if (async)
                        {
                            dataEvents.Add(SixnetCascadingDeletingAsyncDataEvent.Create(dataClient, dataCommand));
                        }
                        else
                        {
                            dataEvents.Add(SixnetCascadingDeletingDataEvent.Create(dataClient, dataCommand));
                        }
                        break;
                    case SixnetDataOperationType.Exist:
                        dataEvents.Add(SixnetCheckingDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Scalar:
                        dataEvents.Add(SixnetGettingValueEvent.Create(dataClient, dataCommand));
                        break;
                }
            }
            return PublishAsync(dataEvents, cancellationToken, dataCommand.GetEntityType());
        }

        /// <summary>
        /// Publish executed data event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        internal static Task PublishExecutedDataEventAsync(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, CancellationToken cancellationToken)
        {
            var dataEvents = new List<ISixnetEvent>();
            if (dataClient != null && dataCommand != null)
            {
                switch (dataCommand.OperationType)
                {
                    case SixnetDataOperationType.Insert:
                        dataEvents.Add(SixnetAddedDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Update:
                        dataEvents.Add(SixnetUpdatedDataEvent.Create(dataClient, dataCommand));
                        break;
                    case SixnetDataOperationType.Delete:
                        dataEvents.Add(SixnetDeletedDataEvent.Create(dataClient, dataCommand));
                        break;
                }
            }
            return PublishAsync(dataEvents, cancellationToken, dataCommand.GetEntityType());
        }

        /// <summary>
        /// Publish queried event
        /// </summary>
        /// <typeparam name="TData">Data type</typeparam>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="datas">Datas</param>
        internal static Task PublishQueriedEventAsync<TData>(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, IEnumerable<TData> datas, CancellationToken cancellationToken)
        {
            return PublishAsync(SixnetQueriedDataEvent<TData>.Create(dataClient, dataCommand, datas), cancellationToken);
        }

        /// <summary>
        /// Publish got value event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="value">Value</param>
        internal static Task PublishGotValueEventAsync(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, dynamic value, CancellationToken cancellationToken)
        {
            return PublishAsync(SixnetGotValueEvent.Create(dataClient, dataCommand, value), cancellationToken);
        }

        /// <summary>
        /// Publish checked event
        /// </summary>
        /// <param name="dataClient">Data client</param>
        /// <param name="dataCommand">Data command</param>
        /// <param name="hasValue">Has value</param>
        internal static Task PublishCheckedEventAsync(ISixnetDataClient dataClient, SixnetDataCommand dataCommand, bool hasValue, CancellationToken cancellationToken)
        {
            return PublishAsync(SixnetCheckedDataEvent.Create(dataClient, dataCommand, hasValue), cancellationToken);
        }

        /// <summary>
        /// Handle work completed handler
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <returns></returns>
        internal static Task PublishWorkCompletedEventAsync(IEnumerable<ISixnetEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            return timeDataEventManager.PublishWorkCompletedEventAsync(eventDatas, cancellationToken);
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handler">Event handler</param>
        public static void SubscribeAll(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handler, configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Action<ISixnetEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Func<ISixnetEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure), configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeAll(handlerExecutor, configure);
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void Subscribe(Type eventType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.Subscribe(eventType, handler, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handlerType">Event handler type</param>
        /// <param name="methodName">Method name</param>
        /// <param name="configure">Configure</param>
        internal static void Subscribe(Type eventType, Type handlerType, string methodName, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.Subscribe(eventType, handlerType, methodName, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void Subscribe<TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe<TEvent>(handler, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handler">Event handler</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe<TEvent>(handler, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        #endregion

        #region Model overall

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeModelAll(Type modelType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll(modelType, handler, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handler">Event handler</param>
        public static void SubscribeModelAll<TModel>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll(typeof(TModel), handler, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeModelAll<TModel>(Action<ISixnetEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeModelAll<TModel>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModelAll<TModel>(Func<ISixnetEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeModelAll<TModel>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModelAll<TModel>(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll(typeof(TModel), SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure), configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all events
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        #endregion

        #region Model specific event

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeModel(Type modelType, Type eventType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            timeDataEventManager.SubscribeModel(modelType, eventType, handler, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public static void SubscribeModel<TEvent>(Type modelType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeModel(modelType, typeof(TEvent), handler, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void SubscribeModel<TModel, TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeModel<TEvent>(typeof(TModel), handler, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModel<TModel, TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeModel<TModel, TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public static void SubscribeModel<TModel, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public static void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model specific event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            timeDataEventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
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
                        if (relationItem.Value.Any(c => (c.Value.Behavior & SixnetRelationBehavior.CascadingDelete) == SixnetRelationBehavior.CascadingDelete))
                        {
                            var relationEntityTypeGuid = relationItem.Key;
                            var relationEntityConfig = SixnetEntityManager.GetEntityConfig(relationEntityTypeGuid);

                            var deletingHandler = Activator.CreateInstance(typeof(SixnetDefaultCascadingDeletingEventHandler<>).MakeGenericType(entityConfig.EntityType)) as ISixnetEventHandler;
                            SubscribeModel(relationEntityConfig.EntityType, typeof(SixnetCascadingDeletingDataEvent), deletingHandler);

                            var asyncDeletingHandler = Activator.CreateInstance(typeof(SixnetDefaultCascadingDeletingAsyncEventHandler<>).MakeGenericType(entityConfig.EntityType)) as ISixnetEventHandler;
                            SubscribeModel(relationEntityConfig.EntityType, typeof(SixnetCascadingDeletingAsyncDataEvent), asyncDeletingHandler);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
