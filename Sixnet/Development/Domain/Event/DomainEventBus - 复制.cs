//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Sixnet.Development.Events;
//using Sixnet.Development.Work;

//namespace Sixnet.Development.Domain.Event
//{
//    /// <summary>
//    /// Domain event bus
//    /// </summary>
//    public static class DomainEventBus
//    {
//        #region Field

//        /// <summary>
//        /// All domain event manager
//        /// </summary>
//        internal static SixnetEventManager AllDomainEventManager = null;
//        /// <summary>
//        /// Trigger time domain event manager
//        /// </summary>
//        internal static Dictionary<EventTriggerTime, SixnetEventManager> TriggerTimeDomainEventManager = null;

//        #endregion

//        #region Constructor

//        static DomainEventBus()
//        {
//            AllDomainEventManager = new SixnetEventManager();
//            TriggerTimeDomainEventManager = new Dictionary<EventTriggerTime, SixnetEventManager>();
//        }

//        #endregion

//        #region Publish

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public static void Publish(params IDomainEvent[] domainEvents)
//        {
//            PublishAsync(domainEvents).Wait();
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public static void Publish(IEnumerable<IDomainEvent> domainEvents)
//        {
//            PublishAsync(domainEvents).Wait();
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public static async Task PublishAsync(params IDomainEvent[] domainEvents)
//        {
//            IEnumerable<IDomainEvent> eventCollection = domainEvents;
//            await PublishAsync(eventCollection).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public static async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
//        {
//            if (!domainEvents.IsNullOrEmpty())
//            {
//                await AllDomainEventManager.PublishAsync(domainEvents).ConfigureAwait(false);
//                UnitOfWork.PublishDomainEvent(domainEvents);
//            }
//        }

//        #endregion

//        #region Subscribe

//        #region Subscribe special domain event

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent>(Action<TEvent> handlerAction, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe(handlerAction, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent>(Func<TEvent, DomainEventResult> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent, THandler>(Expression<Action<TEvent>> handlerAction, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe<TEvent, THandler>(handlerAction, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent, THandler>(Expression<Func<TEvent, DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe<TEvent, THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent>(Func<TEvent, Task> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent>(Func<TEvent, Task<DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent, THandler>(Expression<Func<TEvent, Task>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe<TEvent, THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<TEvent, THandler>(Expression<Func<TEvent, Task<DomainEventResult>>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe<TEvent, THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="eventHandler">Event handler</param>
//        /// <param name="triggerTime">Trigger time</param>
//        public static void Subscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
//        {
//            AllDomainEventManager.Subscribe(eventHandler);
//        }

//        #endregion

//        #region Subscribe all domain event

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe(Action<IDomainEvent> handlerAction, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe(handlerAction, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe(Func<IDomainEvent, DomainEventResult> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<THandler>(Expression<Action<IDomainEvent>> handlerAction, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe<THandler>(handlerAction, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<THandler>(Expression<Func<IDomainEvent, DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe<THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe(Func<IDomainEvent, Task> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe(Func<IDomainEvent, Task<DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<THandler>(Expression<Func<IDomainEvent, Task>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe<THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public static void Subscribe<THandler>(Expression<Func<IDomainEvent, Task<DomainEventResult>>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            AllDomainEventManager.Subscribe<THandler>(handlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event in global area
//        /// </summary>
//        /// <param name="eventHandler">Event handler</param>
//        public static void Subscribe(IDomainEventHandler eventHandler)
//        {
//            AllDomainEventManager.Subscribe(eventHandler);
//        }

//        #endregion

//        #endregion

//        #region Trigger event

//        /// <summary>
//        /// Trigger domain event
//        /// </summary>
//        /// <param name="triggerTime">Trigger time</param>
//        /// <param name="domainEvents">Domain events</param>
//        internal static async Task TriggerEventAsync(EventTriggerTime triggerTime, params IDomainEvent[] domainEvents)
//        {
//            IEnumerable<IDomainEvent> eventCollection = domainEvents;
//            await TriggerEventAsync(triggerTime, eventCollection).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Trigger domain event
//        /// </summary>
//        /// <param name="triggerTime">Trigger time</param>
//        /// <param name="domainEvents">Domain events</param>
//        internal static async Task TriggerEventAsync(EventTriggerTime triggerTime, IEnumerable<IDomainEvent> domainEvents)
//        {
//            await AllDomainEventManager.TriggerEventAsync(triggerTime, domainEvents).ConfigureAwait(false);
//        }

//        #endregion
//    }
//}
