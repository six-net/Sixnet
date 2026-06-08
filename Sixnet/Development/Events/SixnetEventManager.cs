// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Event manager
    /// </summary>
    internal class SixnetEventManager
    {
        #region Fields

        /// <summary>
        /// Overall event handlers
        /// </summary>
        readonly List<ISixnetEventHandler> overallEventHandlers = new();

        /// <summary>
        /// Specific event handlers
        /// key:event type id
        /// </summary>
        readonly Dictionary<Guid, List<ISixnetEventHandler>> specificEventHandlers = new();

        /// <summary>
        /// Model overall event handlers
        /// Key: model type gud
        /// </summary>
        readonly Dictionary<Guid, List<ISixnetEventHandler>> modelOverallEventHandlers = new();

        /// <summary>
        /// Model specific event handlers
        /// Key:entity type id->entity type id
        /// </summary>
        readonly Dictionary<Guid, Dictionary<Guid, List<ISixnetEventHandler>>> modelSpecificEventHandlers = new();

        #endregion

        #region Publish

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        public Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default, Type modelType = null) where TEvent : ISixnetEvent
        {
            return PublishAsync(new TEvent[1] { eventData }, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="eventDatas">Events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public Task PublishAsync<TEvent>(IEnumerable<TEvent> eventDatas, CancellationToken cancellationToken = default, Type modelType = null) where TEvent : ISixnetEvent
        {
            return TriggerEventAsync(eventDatas, cancellationToken, modelType);
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public void SubscribeAll<TEventHanlder>(TEventHanlder handler) where TEventHanlder : ISixnetEventHandler
        {
            if (handler != null)
            {
                overallEventHandlers.Add(handler);
            }
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
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
        /// <param name="handlerExecutor">Handler executor</param>
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
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll(SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure));
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll<ISixnetEvent>(handlerType, handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeAll<ISixnetEvent>(handlerType, handlerExecutorName, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeAll<TEvent>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeAll(handlerType, handlerExecutor.GetLastMemberName(), configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeAll<TEvent>(Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeAll(GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutorName, configure));
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public void Subscribe(Type eventType, ISixnetEventHandler handler)
        {
            if (handler == null || eventType == null)
            {
                return;
            }
            SixnetDirectThrower.ThrowSixnetExceptionIf(!typeof(ISixnetEvent).IsAssignableFrom(eventType), $"{nameof(eventType.FullName)} not implementation {nameof(ISixnetEvent)}");
            if (!specificEventHandlers.TryGetValue(eventType.GUID, out List<ISixnetEventHandler> eventHandlers) || eventHandlers == null)
            {
                eventHandlers = new List<ISixnetEventHandler>();
            }
            eventHandlers.Add(handler);
            specificEventHandlers[eventType.GUID] = eventHandlers;
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
            var handler = GetEventHandlerByExecutorName(eventType, handlerType, methodName, configure);
            Subscribe(eventType, handler);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void Subscribe<TEvent>(ISixnetEventHandler handler) where TEvent : ISixnetEvent
        {
            Subscribe(typeof(TEvent), handler);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            Subscribe<TEvent>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            };
            Subscribe<TEvent>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure));
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure));
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent>(ISixnetEventHandler handler, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            if (handler == null || (applyToModel && modelTypes.IsNullOrEmpty()))
            {
                return;
            }
            if (applyToModel)
            {
                foreach (var modelType in modelTypes)
                {
                    SubscribeModel<TEvent>(modelType, handler);
                }
            }
            else
            {
                Subscribe<TEvent>(handler);
            }
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            };
            Subscribe<TEvent>(asyncHandlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            };
            Subscribe<TEvent>(asyncHandlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure), applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure, applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : class, ISixnetEvent
        {
            Subscribe<TEvent>(GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure), applyToModel, modelTypes);
        }

        #endregion

        #region Model overall

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModelAll(Type modelType, ISixnetEventHandler handler)
        {
            if (modelType == null || handler == null)
            {
                return;
            }
            if (!modelOverallEventHandlers.TryGetValue(modelType.GUID, out var eventHandlers) || eventHandlers.IsNullOrEmpty())
            {
                eventHandlers = new List<ISixnetEventHandler>();
            }
            eventHandlers.Add(handler);
            modelOverallEventHandlers[modelType.GUID] = eventHandlers;
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handler">Event handler</param>
        public void SubscribeModelAll<TModel>(ISixnetEventHandler handler)
        {
            SubscribeModelAll(typeof(TModel), handler);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
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
        /// <param name="handlerExecutor">Handler excutor</param>
        /// <param name="configure">Configure handler options</param>
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
        /// <param name="handlerExecutor">Handler excutor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel>(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(SixnetDefaultEventHandler.GetDefaultEventHandler<ISixnetEvent>(handlerExecutor, configure));
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel, ISixnetEvent>(handlerType, handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel>(Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel, ISixnetEvent>(handlerType, handlerExecutorName, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeModelAll<TModel, TEvent>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModelAll<TModel, TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeModelAll<TModel, TEvent>(Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModelAll<TModel>(SixnetEventManager.GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutorName, configure));
        }

        #endregion

        #region Model specific event

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel(Type modelType, Type eventType, ISixnetEventHandler handler)
        {
            if (modelType == null || eventType == null || handler == null)
            {
                return;
            }
            if (!typeof(ISixnetEvent).IsAssignableFrom(eventType))
            {
                throw new SixnetException($"{nameof(eventType.FullName)} not implementation {nameof(ISixnetEvent)}");
            }
            if (!modelSpecificEventHandlers.TryGetValue(modelType.GUID, out Dictionary<Guid, List<ISixnetEventHandler>> entityEventHandlers) || entityEventHandlers == null)
            {
                entityEventHandlers = new Dictionary<Guid, List<ISixnetEventHandler>>();
            }
            if (!entityEventHandlers.TryGetValue(eventType.GUID, out var eventHandlers) || eventHandlers.IsNullOrEmpty())
            {
                eventHandlers = new List<ISixnetEventHandler>();
            }
            eventHandlers.Add(handler);
            entityEventHandlers[eventType.GUID] = eventHandlers;
            modelSpecificEventHandlers[modelType.GUID] = entityEventHandlers;
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="modelType">Model type</param>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel<TEvent>(Type modelType, ISixnetEventHandler handler)
        {
            if (modelType == null || handler == null)
            {
                return;
            }
            var eventType = typeof(TEvent);
            SubscribeModel(modelType, eventType, handler);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public void SubscribeModel<TModel, TEvent>(ISixnetEventHandler handler) where TEvent : ISixnetEvent
        {
            SubscribeModel<TEvent>(typeof(TModel), handler);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModel<TModel, TEvent>(Action<TEvent> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeModel<TModel, TEvent>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            Task asyncHandlerExecutor(TEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeModel<TModel, TEvent>(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class, ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(SixnetDefaultEventHandler.GetDefaultEventHandler<TEvent>(handlerExecutor, configure));
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent>(Type handlerType, Expression handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(SixnetEventManager.GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure));
        }

        #endregion

        #endregion

        #region Trigger

        /// <summary>
        /// Trigger event
        /// </summary>
        /// <param name="events">Events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        async Task TriggerEventAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken, Type modelType = null) where TEvent : ISixnetEvent
        {
            if (events.IsNullOrEmpty())
            {
                return;
            }
            foreach (var eventData in events)
            {
                //overall
                foreach (var h in overallEventHandlers)
                {
                    await h.Handle(eventData, cancellationToken).ConfigureAwait(false);
                }

                //specific event
                var eventType = eventData.GetType();
                if (specificEventHandlers.TryGetValue(eventType.GUID, out var currentSpecificEventHandlers) && !currentSpecificEventHandlers.IsNullOrEmpty())
                {
                    foreach (var h in currentSpecificEventHandlers)
                    {
                        await h.Handle(eventData, cancellationToken).ConfigureAwait(false);
                    }
                }

                if (modelType != null)
                {
                    //model overall
                    if (modelOverallEventHandlers.TryGetValue(modelType.GUID, out var modelOverallHandlers) && !modelOverallHandlers.IsNullOrEmpty())
                    {
                        foreach (var h in modelOverallHandlers)
                        {
                            await h.Handle(eventData, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    //model specitic event
                    if (modelSpecificEventHandlers.TryGetValue(modelType.GUID, out var currentModelSpecificHandlers) && !currentModelSpecificHandlers.IsNullOrEmpty())
                    {
                        if (currentModelSpecificHandlers.TryGetValue(eventType.GUID, out var modelSpecificEventHandlers) && !modelSpecificEventHandlers.IsNullOrEmpty())
                        {
                            foreach (var h in modelSpecificEventHandlers)
                            {
                                await h.Handle(eventData, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Util

        /// <summary>
        /// Get event handler by executor name
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <returns></returns>
        internal static ISixnetEventHandler GetEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            var handlerExecutor = GetEventHandlerExecutor(typeof(TEvent), handlerType, handlerExecutorName);
            return SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure);
        }

        /// <summary>
        /// Get event handler by executor name
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <returns></returns>
        internal static ISixnetEventHandler GetEventHandlerByExecutorName(Type eventType, Type handlerType, string handlerExecutorName, Action<SixnetEventHandlerOptions> configure = null)
        {
            var handlerExecutor = GetEventHandlerExecutor(eventType, handlerType, handlerExecutorName);
            return SixnetDefaultEventHandler.GetDefaultEventHandler(handlerExecutor, configure);
        }

        /// <summary>
        /// Get event handler executor
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handlerType"></param>
        /// <param name="handlerExecutorName"></param>
        internal static Func<object, CancellationToken, Task> GetEventHandlerExecutor(Type eventType, Type handlerType, string handlerExecutorName)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(handlerType == null, $"{nameof(handlerType)} is null");
            SixnetDirectThrower.ThrowSixnetExceptionIf(string.IsNullOrWhiteSpace(handlerExecutorName), $"{nameof(handlerExecutorName)} is null or empty");

            var handler = SixnetContainer.GetService(handlerType);
            SixnetDirectThrower.ThrowSixnetExceptionIf(handler == null, $"Can't resolve {handlerType.FullName}");

            var executorMember = handler.GetType().GetMethods().FirstOrDefault(c =>
            {
                var memberParameters = c.GetParameters();
                return !memberParameters.IsNullOrEmpty() && eventType == memberParameters.First().ParameterType;
            });
            SixnetDirectThrower.ThrowSixnetExceptionIf(executorMember == null, $"Not found {handlerExecutorName} in {handlerType.FullName} for event {eventType.FullName}");

            var parameters = executorMember.GetParameters();
            var returnType = executorMember.ReturnType;
            Func<object, CancellationToken, Task> handlerExeutor = null;
            var eventParam = Expression.Parameter(typeof(object), "e");
            var castEvent = Expression.Convert(eventParam, eventType);
            var tokenParam = Expression.Parameter(typeof(CancellationToken), "ct");

            if (returnType == null)
            {
                var handlerAction = Expression.Call(Expression.Constant(handler), executorMember, castEvent);
                var handlerActionCall = Expression.Lambda<Action<object>>(handlerAction, eventParam).Compile();
                handlerExeutor = (e, ct) =>
                {
                    handlerActionCall.Invoke(e);
                    return Task.CompletedTask;
                };
            }
            else if (returnType == typeof(Task))
            {
                if (parameters.Length == 1)
                {
                    var handlerAction = Expression.Call(Expression.Constant(handler), executorMember, castEvent);
                    var handlerActionCall = Expression.Lambda<Func<object, Task>>(handlerAction, eventParam).Compile();
                    handlerExeutor = (e, ct) =>
                    {
                        return handlerActionCall.Invoke(e);
                    };
                }
                if (parameters.Length == 2 && parameters[1].ParameterType == typeof(CancellationToken))
                {
                    var handlerAction = Expression.Call(Expression.Constant(handler), executorMember, castEvent, tokenParam);
                    handlerExeutor = Expression.Lambda<Func<object, CancellationToken, Task>>(handlerAction, eventParam, tokenParam).Compile();

                }
            }

            SixnetDirectThrower.ThrowSixnetExceptionIf(handlerExeutor == null, $"{handlerExecutorName} not an event handling method ");
            return handlerExeutor;
        }

        #endregion
    }
}
