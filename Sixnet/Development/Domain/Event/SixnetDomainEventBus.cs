using Sixnet.Development.Data.Event;
using Sixnet.Development.Event;
using Sixnet.Development.Work;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Domain event bus
    /// </summary>
    public static class SixnetDomainEventBus
    {
        #region Fields

        static readonly TimeEventManager _timeDomainEventManager = new();

        #endregion

        #region Publish

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task Publish(ISixnetDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            return Publish(new ISixnetDomainEvent[1] { domainEvent }, cancellationToken);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task Publish(IEnumerable<ISixnetDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            var publishTask = _timeDomainEventManager.Publish(domainEvents, cancellationToken);
            UnitOfWork.PublishDomainEvent(domainEvents);
            return publishTask;
        }

        #endregion

        #region Subscribe

        #region Overall

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handler">Event handler</param>
        /// <param name="configure">Configure options</param>
        public static void SubscribeAll(ISixnetDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null)
        {
            _timeDomainEventManager.SubscribeAll(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Action<ISixnetDomainEvent> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDomainEvent e, CancellationToken ct)
            {
                handlerExecutor(e);
                return Task.CompletedTask;
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Func<ISixnetDomainEvent, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(ISixnetDomainEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll(Func<ISixnetDomainEvent, CancellationToken, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            _timeDomainEventManager.SubscribeAll(new DefaultDomainEventHandler<ISixnetDomainEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Action<ISixnetDomainEvent>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            _timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetDomainEvent, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            _timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<ISixnetDomainEvent, CancellationToken, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            _timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<ISixnetDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void Subscribe(Type eventType, ISixnetDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null)
        {
            _timeDomainEventManager.Subscribe(eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void Subscribe<TEvent>(ISixnetDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe<TEvent>(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, ISixnetDomainEvent
        {
            _timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        #endregion

        #endregion

        #region Handle

        /// <summary>
        /// Handle work completed handler
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <returns></returns>
        internal static Task HandleWorkCompleted(IEnumerable<ISixnetDomainEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            return _timeDomainEventManager.HandleWorkCompleted(eventDatas, cancellationToken);
        }

        #endregion

        #region Util

        static Action<TimeEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(DomainEventHandlerOptions options)
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

        static Action<TimeEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(Action<DomainEventHandlerOptions> configure)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            return o =>
            {
                o.Async = options.Async;
                o.TriggerTime = options.TriggerTime;
            };
        }

        static DomainEventHandlerOptions GetConfigurationDataEventHandlerOptions(Action<DomainEventHandlerOptions> configure)
        {
            var options = new DomainEventHandlerOptions();
            configure?.Invoke(options);
            return options;
        }

        /// <summary>
        /// Get event handler by executor name
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="handlerExecutorName">Handler executor name</param>
        /// <returns></returns>
        static ISixnetDomainEventHandler GetDataEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, DomainEventHandlerOptions options = null) where TEvent : ISixnetDomainEvent
        {
            var handlerExecutor = EventManager.GetEventHandlerExecutor<TEvent>(handlerType, handlerExecutorName);
            return new DefaultDomainEventHandler<TEvent>(handlerExecutor, options);
        }

        #endregion
    }
}
