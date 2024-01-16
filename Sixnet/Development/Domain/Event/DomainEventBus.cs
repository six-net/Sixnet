using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Event;
using Sixnet.Development.Data;
using Sixnet.Development.Entity;
using Sixnet.Development.Events;
using Sixnet.Development.Work;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Domain event bus
    /// </summary>
    public static class DomainEventBus
    {
        #region Fields

        internal static readonly TimeSixnetEventManager timeDomainEventManager = null;

        #endregion

        #region Constructor

        static DomainEventBus()
        {
            timeDomainEventManager = new TimeSixnetEventManager();
        }

        #endregion

        #region Publish

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            return Publish(new IDomainEvent[1] { domainEvent }, cancellationToken);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            var publishTask = timeDomainEventManager.Publish(domainEvents, cancellationToken);
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
        public static void SubscribeAll(IDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null)
        {
            timeDomainEventManager.SubscribeAll(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Event handler</param>
        /// <param name="configure">Configure handler options</param>
        public static void SubscribeAll(Action<IDomainEvent> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDomainEvent e, CancellationToken ct)
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
        public static void SubscribeAll(Func<IDomainEvent, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            Task asyncHandlerExecutor(IDomainEvent e, CancellationToken ct)
            {
                return handlerExecutor(e);
            }
            SubscribeAll(asyncHandlerExecutor, configure);
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll(Func<IDomainEvent, CancellationToken, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDomainEventManager.SubscribeAll(new DefaultDomainEventHandler<IDomainEvent>(handlerExecutor, options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Action<IDomainEvent>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<IDomainEvent, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void SubscribeAll<THandler>(Expression<Func<THandler, Func<IDomainEvent, CancellationToken, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null)
        {
            var options = GetConfigurationDataEventHandlerOptions(configure);
            timeDomainEventManager.SubscribeAll(GetDataEventHandlerByExecutorName<IDomainEvent>(typeof(THandler), handlerExecutor.GetLastMemberName(), options), GetTimeSixnetEventHandlerOptionsConfigure(options));
        }

        #endregion

        #region Specific event

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        public static void Subscribe(Type eventType, IDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null)
        {
            timeDomainEventManager.Subscribe(eventType, handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        public static void Subscribe<TEvent>(IDataEventHandler handler, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe<TEvent>(handler, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Action<TEvent> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe domain event
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Action<TEvent>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        /// <summary>
        /// Subscribe all domain event
        /// </summary>
        /// <param name="handlerExecutor">Handler executor</param>
        public static void Subscribe<TEvent, THandler>(Expression<Func<THandler, Func<TEvent, CancellationToken, Task>>> handlerExecutor, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
        {
            timeDomainEventManager.Subscribe(handlerExecutor, GetTimeSixnetEventHandlerOptionsConfigure(configure));
        }

        #endregion

        #endregion

        #region Handle

        /// <summary>
        /// Handle work completed handler
        /// </summary>
        /// <param name="eventDatas">Event datas</param>
        /// <returns></returns>
        internal static Task HandleWorkCompleted(IEnumerable<IDomainEvent> eventDatas, CancellationToken cancellationToken = default)
        {
            return timeDomainEventManager.HandleWorkCompleted(eventDatas, cancellationToken);
        }

        #endregion

        #region Util

        static Action<TimeSixnetEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(DomainEventHandlerOptions options)
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

        static Action<TimeSixnetEventHandlerOptions> GetTimeSixnetEventHandlerOptionsConfigure(Action<DomainEventHandlerOptions> configure)
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
        static IDomainEventHandler GetDataEventHandlerByExecutorName<TEvent>(Type handlerType, string handlerExecutorName, DomainEventHandlerOptions options = null) where TEvent : IDomainEvent
        {
            var handlerExecutor = SixnetEventManager.GetEventHandlerExecutor<TEvent>(handlerType, handlerExecutorName);
            return new DefaultDomainEventHandler<TEvent>(handlerExecutor, options);
        }

        #endregion
    }
}
