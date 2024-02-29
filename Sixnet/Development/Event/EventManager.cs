using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Event
{
    /// <summary>
    /// Event manager
    /// </summary>
    internal class EventManager
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
        public Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken = default, Type modelType = null) where TEvent : ISixnetEvent
        {
            return Publish(new TEvent[1] { eventData }, cancellationToken, modelType);
        }

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="eventDatas">Events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public Task Publish<TEvent>(IEnumerable<TEvent> eventDatas, CancellationToken cancellationToken = default, Type modelType = null) where TEvent : ISixnetEvent
        {
            return TriggerEvent(eventDatas, cancellationToken, modelType);
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
        public void SubscribeAll(Action<ISixnetEvent> handlerExecutor, Action<EventHandlerOptions> configure = null)
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
        public void SubscribeAll(Func<ISixnetEvent, Task> handlerExecutor, Action<EventHandlerOptions> configure = null)
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
        public void SubscribeAll(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            var options = new EventHandlerOptions();
            configure?.Invoke(options);
            SubscribeAll(new DefaultEventHandler<ISixnetEvent>(handlerExecutor, options));
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeAll(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeAll<ISixnetEvent>(handlerType, handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeAll(Type handlerType, string handlerExecutorName, Action<EventHandlerOptions> configure = null)
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
        internal void SubscribeAll<TEvent>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeAll(handlerType, handlerExecutor.GetLastMemberName(), configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeAll<TEvent>(Type handlerType, string handlerExecutorName, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            var options = new EventHandlerOptions();
            configure?.Invoke(options);

            Subscribe<TEvent>(new DefaultEventHandler<TEvent>(handlerExecutor, options));
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            Subscribe<TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void Subscribe<TEvent>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(ISixnetEventHandler handler, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
        {
            var options = new EventHandlerOptions();
            configure?.Invoke(options);

            Subscribe<TEvent>(new DefaultEventHandler<TEvent>(handlerExecutor, options), applyToModel, modelTypes);
        }

        /// <summary>
        /// Subscibe event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        /// <param name="applyToModel">Whether apply to model</param>
        /// <param name="modelTypes">Model types</param>
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void Subscribe<TEvent>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null, bool applyToModel = false, params Type[] modelTypes) where TEvent : ISixnetEvent
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
        public void SubscribeModelAll<TModel>(Action<ISixnetEvent> handlerExecutor, Action<EventHandlerOptions> configure = null)
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
        public void SubscribeModelAll<TModel>(Func<ISixnetEvent, Task> handlerExecutor, Action<EventHandlerOptions> configure = null)
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
        public void SubscribeModelAll<TModel>(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            var options = new EventHandlerOptions();
            configure?.Invoke(options);

            SubscribeModelAll<TModel>(new DefaultEventHandler<ISixnetEvent>(handlerExecutor, options));
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Action<ISixnetEvent>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel, THandler>(Expression<Func<THandler, Func<ISixnetEvent, CancellationToken, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutor">Handler executor</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null)
        {
            SubscribeModelAll<TModel, ISixnetEvent>(handlerType, handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        public void SubscribeModelAll<TModel>(Type handlerType, string handlerExecutorName, Action<EventHandlerOptions> configure = null)
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
        internal void SubscribeModelAll<TModel, TEvent>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModelAll<TModel, TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure);
        }

        /// <summary>
        /// Subscribe model all event
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <param name="configure">Configure handler options</param>
        internal void SubscribeModelAll<TModel, TEvent>(Type handlerType, string handlerExecutorName, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModelAll<TModel>(GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutorName, configure));
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
        public void SubscribeModel<TModel, TEvent>(Action<TEvent> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, Task> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
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
        public void SubscribeModel<TModel, TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            var options = new EventHandlerOptions();
            configure?.Invoke(options);

            SubscribeModel<TModel, TEvent>(new DefaultEventHandler<TEvent>(handlerExecutor, options));
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(typeof(THandler), handlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe model event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public void SubscribeModel<TModel, TEvent>(Type handlerType, Expression handlerExecutor, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            SubscribeModel<TModel, TEvent>(GetEventHandlerByExecutorName<TEvent>(handlerType, handlerExecutor.GetLastMemberName(), configure));
        }

        #endregion

        #endregion

        #region Trigger event

        /// <summary>
        /// Trigger event
        /// </summary>
        /// <param name="events">Events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        Task TriggerEvent<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken, Type modelType = null) where TEvent : ISixnetEvent
        {
            if (events.IsNullOrEmpty())
            {
                return Task.CompletedTask;
            }
            var eventTasks = new List<Task>();
            foreach (var eventData in events)
            {
                //overall
                eventTasks.AddRange(overallEventHandlers.Select(h => h.Handle(eventData, cancellationToken)));

                //specific event
                var eventType = eventData.GetType();
                if (specificEventHandlers.TryGetValue(eventType.GUID, out var currentSpecificEventHandlers) && !currentSpecificEventHandlers.IsNullOrEmpty())
                {
                    eventTasks.AddRange(currentSpecificEventHandlers.Select(h => h.Handle(eventData, cancellationToken)));
                }

                if (modelType != null)
                {
                    //model overall
                    if (modelOverallEventHandlers.TryGetValue(modelType.GUID, out var modelOverallHandlers) && !modelOverallHandlers.IsNullOrEmpty())
                    {
                        eventTasks.AddRange(modelOverallHandlers.Select(h => h.Handle(eventData, cancellationToken)));
                    }
                    //model specitic event
                    if (modelSpecificEventHandlers.TryGetValue(modelType.GUID, out var currentModelSpecificHandlers) && !currentModelSpecificHandlers.IsNullOrEmpty())
                    {
                        if (currentModelSpecificHandlers.TryGetValue(eventType.GUID, out var modelSpecificEventHandlers) && !modelSpecificEventHandlers.IsNullOrEmpty())
                        {
                            eventTasks.AddRange(modelSpecificEventHandlers.Select(h => h.Handle(eventData, cancellationToken)));
                        }
                    }
                }
            }
            return eventTasks.IsNullOrEmpty()
                ? Task.CompletedTask
                : Task.WhenAll(eventTasks);
        }

        #endregion

        #region Util

        /// <summary>
        /// Get event handler by executor name
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <returns></returns>
        internal ISixnetEventHandler GetEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, Action<EventHandlerOptions> configure = null) where TEvent : ISixnetEvent
        {
            var handlerExecutor = GetEventHandlerExecutor<TEvent>(handlerType, handlerExecutorName);

            var options = new EventHandlerOptions();
            configure?.Invoke(options);

            return new DefaultEventHandler<TEvent>(handlerExecutor, options);
        }

        /// <summary>
        /// Get event handler executor
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handlerType"></param>
        /// <param name="handlerExecutorName"></param>
        internal static Func<TEvent, CancellationToken, Task> GetEventHandlerExecutor<TEvent>(Type handlerType, string handlerExecutorName) where TEvent : ISixnetEvent
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(handlerType == null, $"{nameof(handlerType)} is null");
            SixnetDirectThrower.ThrowSixnetExceptionIf(string.IsNullOrWhiteSpace(handlerExecutorName), $"{nameof(handlerExecutorName)} is null or empty");

            var handler = SixnetContainer.GetService(handlerType);
            SixnetDirectThrower.ThrowSixnetExceptionIf(handler == null, $"Can't resolve {handlerType.FullName}");

            var executorMember = handler.GetType().GetMethods().FirstOrDefault(c => c.Name == handlerExecutorName);
            SixnetDirectThrower.ThrowSixnetExceptionIf(executorMember == null, $"Not found {handlerExecutorName} in {handlerType.FullName}");

            var parameters = executorMember.GetParameters();
            var eventTypeContract = typeof(TEvent);
            SixnetDirectThrower.ThrowSixnetExceptionIf(parameters.IsNullOrEmpty() || eventTypeContract != parameters.First().ParameterType, $"{handlerExecutorName} not an event handling method ");

            var returnType = executorMember.ReturnType;
            Func<TEvent, CancellationToken, Task> handlerExeutor = null;
            if (returnType == null)
            {
                var handlerAction = Delegate.CreateDelegate(typeof(Action<TEvent>), handler, executorMember) as Action<TEvent>;
                handlerExeutor = (e, ct) =>
                {
                    handlerAction.Invoke(e);
                    return Task.CompletedTask;
                };
            }
            else if (returnType == typeof(Task))
            {
                if (parameters.Length == 1)
                {
                    var handlerAction = Delegate.CreateDelegate(typeof(Func<TEvent, Task>), handler, executorMember) as Func<TEvent, Task>;
                    handlerExeutor = (e, ct) =>
                    {
                        return handlerAction.Invoke(e);
                    };
                }
                if (parameters.Length == 2 && parameters[1].ParameterType == typeof(CancellationToken))
                {
                    handlerExeutor = Delegate.CreateDelegate(typeof(Func<TEvent, CancellationToken, Task>), handler, executorMember) as Func<TEvent, CancellationToken, Task>;
                }
            }

            SixnetDirectThrower.ThrowSixnetExceptionIf(handlerExeutor == null, $"{handlerExecutorName} not an event handling method ");
            return handlerExeutor;
        }

        #endregion
    }
}
