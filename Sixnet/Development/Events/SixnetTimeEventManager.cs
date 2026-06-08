// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Trigger time sixnet event manager
    /// </summary>
    internal class SixnetTimeEventManager
    {
        #region Fields

        readonly Dictionary<SixnetEventTriggerTime, SixnetEventManager> _triggerTimeEventManagers = new()
        {
            { SixnetEventTriggerTime.Immediately, new SixnetEventManager() },
            { SixnetEventTriggerTime.WorkCompleted, new SixnetEventManager() }
        };

        #endregion

        #region Publish

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public Task PublishAsync(ISixnetEvent eventData, CancellationToken cancellationToken = default, Type modelType = null)
        {
            return PublishAsync(new ISixnetEvent[1] { eventData }, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public Task PublishAsync(IEnumerable<ISixnetEvent> eventDatas, CancellationToken cancellationToken = default, Type modelType = null)
        {
            var eventManager = GetSixnetEventManager(SixnetEventTriggerTime.Immediately);
            return eventManager.PublishAsync(eventDatas, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish work completed event
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <returns></returns>
        internal Task PublishWorkCompletedEventAsync(IEnumerable<ISixnetEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            if (!eventDatas.IsNullOrEmpty())
            {
                var workCompletedEventManager = GetSixnetEventManager(SixnetEventTriggerTime.WorkCompleted);
                return workCompletedEventManager.PublishAsync(eventDatas, cancellationToken);
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public void SubscribeAll(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.SubscribeAll(handler);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Action<ISixnetEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Func<ISixnetEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeAll(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.SubscribeAll(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.SubscribeAll(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeAll(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeAll(handlerExecutor, configure);
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public void Subscribe(Type eventType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.Subscribe(eventType, handler);
        }

        /// <summary>
        /// Subscribe specific event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handlerType">Event handler type</param>
        /// <param name="methodName">Method name</param>
        /// <param name="configure">Configure</param>
        public void Subscribe(Type eventType, Type handlerType, string methodName, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.Subscribe(eventType, handlerType, methodName, configure);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void Subscribe<TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe<TEvent>(handler);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handler">Event handler</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe<TEvent>(handler, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler action</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Data event</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="applyToModel">Whether apply to entity</param>
        /// <param name="modelTypes">Entity types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);
            eventManager.Subscribe(handlerExecutor, configure, applyToModel, modelTypes);
        }

        #endregion

        #region Model overall

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModelAll(Type modelType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll(modelType, handler);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public void SubscribeModelAll<TModel>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll<TModel>(handler);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        public void SubscribeModelAll<TModel>(Action<ISixnetEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeModelAll<TModel>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModelAll<TModel>(Func<ISixnetEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeModelAll<TModel>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModelAll<TModel>(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll<TModel>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModelAll<TModel, THandler>(handlerExecutor, configure);
        }

        #endregion

        #region Model specific event

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel(Type modelType, Type eventType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel(modelType, eventType, handler);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel<TEvent>(Type modelType, ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeModel(modelType, typeof(TEvent), handler, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel<TModel, TEvent>(ISixnetEventHandler handler, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeModel<TEvent>(typeof(TModel), handler, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler action</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            var options = GetConfigurationEventHandlerOptions(configure);
            var eventManager = GetSixnetEventManager(options.TriggerTime);

            eventManager.SubscribeModel<TModel, TEvent, THandler>(handlerExecutor, configure);
        }

        #endregion

        #endregion

        #region Util

        SixnetEventHandlerOptions GetConfigurationEventHandlerOptions(Action<SixnetEventHandlerOptions> configure)
        {
            var options = new SixnetEventHandlerOptions();
            configure?.Invoke(options);
            return options;
        }

        SixnetEventManager GetSixnetEventManager(SixnetEventTriggerTime triggerTime)
        {
            return _triggerTimeEventManagers[triggerTime];
        }

        #endregion
    }
}
