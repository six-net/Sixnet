//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading;
//using System.Threading.Tasks;
//using Sixnet.DependencyInjection;
//using Sixnet.Development.Data.Event;
//using Sixnet.Exceptions;
//using Sixnet.Expressions.Linq;

//namespace Sixnet.Development.Domain.Event
//{
//    /// <summary>
//    /// Domain event manager
//    /// </summary>
//    public class DomainEventManager
//    {
//        #region Fields

//        /// <summary>
//        /// Special event handlers
//        /// Key: Event type guid
//        /// </summary>
//        readonly Dictionary<Guid, Dictionary<EventTriggerTime, List<IDomainEventHandler>>> specialEventHandlers = new();

//        /// <summary>
//        /// Overall event handlers
//        /// </summary>
//        readonly Dictionary<EventTriggerTime, List<IDomainEventHandler>> overallEventHandlers = new();

//        #endregion

//        #region Publish

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public void Publish(params IDomainEvent[] domainEvents)
//        {
//            PublishAsync(domainEvents).Wait();
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public void Publish(IEnumerable<IDomainEvent> domainEvents)
//        {
//            PublishAsync(domainEvents).Wait();
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public async Task PublishAsync(params IDomainEvent[] domainEvents)
//        {
//            IEnumerable<IDomainEvent> eventCollection = domainEvents;
//            await PublishAsync(eventCollection).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Publish domain event
//        /// </summary>
//        /// <param name="domainEvents">Domain events</param>
//        public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
//        {
//            await TriggerEventAsync(EventTriggerTime.Immediately, domainEvents).ConfigureAwait(false);
//        }

//        #endregion

//        #region Subscribe special domain event

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent>(Action<TEvent> handlerAction, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            Func<TEvent, Task<DomainEventResult>> asyncHandlerFunc = e =>
//            {
//                handlerAction(e);
//                return Task.FromResult(DomainEventResult.EmptyResult());
//            };
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent>(Func<TEvent, DomainEventResult> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            Func<TEvent, Task<DomainEventResult>> asyncHandlerFunc = e => Task.FromResult(handlerFunc(e));
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent, THandler>(Expression<Action<TEvent>> handlerAction, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerAction)), configure);
//            Subscribe<TEvent>(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent, THandler>(Expression<Func<TEvent, DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe<TEvent>(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent>(Func<TEvent, Task> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            Func<TEvent, Task<DomainEventResult>> asyncHandlerFunc = async e =>
//            {
//                await handlerFunc(e).ConfigureAwait(false);
//                return DomainEventResult.EmptyResult();
//            };
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent>(Func<TEvent, Task<DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            var options = new DomainEventHandlerOptions();
//            configure?.Invoke(options);
//            Subscribe<TEvent>(new DefaultDomainEventHandler<TEvent>(handlerFunc, options));
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent, THandler>(Expression<Func<TEvent, Task>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe<TEvent>(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<TEvent, THandler>(Expression<Func<TEvent, Task<DomainEventResult>>> handlerFunc, Action<DomainEventHandlerOptions> configure = null) where TEvent : class, IDomainEvent
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe<TEvent>(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe domain event
//        /// </summary>
//        /// <typeparam name="TEvent">Domain event</typeparam>
//        /// <param name="eventHandler">Event handler</param>
//        /// <param name="triggerTime">Trigger time</param>
//        public void Subscribe<TEvent>(IDomainEventHandler eventHandler) where TEvent : class, IDomainEvent
//        {
//            if (eventHandler != null)
//            {
//                var eventType = typeof(TEvent);
//                var triggerTime = eventHandler.Options?.TriggerTime ?? EventTriggerTime.Immediately;
//                if (!specialEventHandlers.TryGetValue(eventType.GUID, out Dictionary<EventTriggerTime, List<IDomainEventHandler>> executeTimeHandlers) || executeTimeHandlers == null)
//                {
//                    executeTimeHandlers = new Dictionary<EventTriggerTime, List<IDomainEventHandler>>()
//                {
//                    { triggerTime,new List<IDomainEventHandler>(){ eventHandler } }
//                };
//                }
//                else
//                {
//                    if (!executeTimeHandlers.TryGetValue(triggerTime, out var handlers) || handlers == null)
//                    {
//                        executeTimeHandlers[triggerTime] = new List<IDomainEventHandler>() { eventHandler };
//                    }
//                    else
//                    {
//                        handlers.Add(eventHandler);
//                    }
//                }
//                specialEventHandlers[eventType.GUID] = executeTimeHandlers;
//            }
//        }

//        #endregion

//        #region Subscribe all domain event

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe(Action<IDomainEvent> handlerAction, Action<DomainEventHandlerOptions> configure = null)
//        {
//            Func<IDomainEvent, Task<DomainEventResult>> asyncHandlerFunc = e =>
//            {
//                handlerAction(e);
//                return Task.FromResult(DomainEventResult.EmptyResult());
//            };
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe(Func<IDomainEvent, DomainEventResult> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            Func<IDomainEvent, Task<DomainEventResult>> asyncHandlerFunc = e => Task.FromResult(handlerFunc(e));
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerAction">Handler action</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<THandler>(Expression<Action<IDomainEvent>> handlerAction, Action<DomainEventHandlerOptions> configure = null)
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerAction)), configure);
//            Subscribe(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<THandler>(Expression<Func<IDomainEvent, DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe(Func<IDomainEvent, Task> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            Func<IDomainEvent, Task<DomainEventResult>> asyncHandlerFunc = async e =>
//            {
//                await handlerFunc(e).ConfigureAwait(false);
//                return DomainEventResult.EmptyResult();
//            };
//            Subscribe(asyncHandlerFunc, configure);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe(Func<IDomainEvent, Task<DomainEventResult>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            // options
//            var options = new DomainEventHandlerOptions();
//            configure?.Invoke(options);
//            Subscribe(new DefaultDomainEventHandler<IDomainEvent>(handlerFunc, options));
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<THandler>(Expression<Func<IDomainEvent, Task>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe all domain event
//        /// </summary>
//        /// <param name="handlerFunc">Handler func</param>
//        /// <param name="configure">Configure options</param>
//        public void Subscribe<THandler>(Expression<Func<IDomainEvent, Task<DomainEventResult>>> handlerFunc, Action<DomainEventHandlerOptions> configure = null)
//        {
//            var eventHandler = GetDomainEventHandlerByActionName(typeof(THandler), ExpressionHelper.GetExpressionLastPropertyName(ExpressionHelper.GetLastChildExpression(handlerFunc)), configure);
//            Subscribe(eventHandler);
//        }

//        /// <summary>
//        /// Subscribe domain event in global area
//        /// </summary>
//        /// <param name="eventHandler">Event handler</param>
//        public void Subscribe(IDomainEventHandler eventHandler)
//        {
//            if (eventHandler != null)
//            {
//                var triggerTime = eventHandler.Options?.TriggerTime ?? EventTriggerTime.Immediately;
//                if (!overallEventHandlers.TryGetValue(triggerTime, out var handlers) || handlers == null)
//                {
//                    handlers = new List<IDomainEventHandler>() { eventHandler };
//                }
//                else
//                {
//                    handlers.Add(eventHandler);
//                }
//                overallEventHandlers[triggerTime] = handlers;
//            }
//        }

//        #endregion

//        #region Get domain event handler by action name

//        /// <summary>
//        /// Get domain event handler by action name
//        /// </summary>
//        /// <param name="handlerType">Handler type</param>
//        /// <param name="handlerActionName">Handler action name</param>
//        /// <param name="configure">Configure handler options</param>
//        /// <returns></returns>
//        IDomainEventHandler GetDomainEventHandlerByActionName(Type handlerType, string handlerActionName, Action<DomainEventHandlerOptions> configure = null)
//        {
//            ExceptionUtil.ThrowFrameworkErrorIf(handlerType == null, $"{nameof(handlerType)} is null");
//            ExceptionUtil.ThrowFrameworkErrorIf(string.IsNullOrWhiteSpace(handlerActionName), $"{nameof(handlerActionName)} is null");

//            var handler = ContainerManager.Resolve(handlerType);
//            ExceptionUtil.ThrowFrameworkErrorIf(handler == null, $"Can't resolve {handlerType.FullName}");

//            var actionMember = handler.GetType().GetMethods().FirstOrDefault(c => c.Name == handlerActionName);
//            ExceptionUtil.ThrowFrameworkErrorIf(actionMember == null, $"Not found {handlerActionName} in {handlerType.FullName}");

//            var parameters = actionMember.GetParameters();
//            var eventTypeContract = typeof(IDomainEvent);
//            ExceptionUtil.ThrowFrameworkErrorIf(parameters.IsNullOrEmpty() || !eventTypeContract.IsAssignableFrom(parameters.First().ParameterType), $"{handlerActionName} not an event handling method ");

//            var returnType = actionMember.ReturnType;
//            var eventResultType = typeof(DomainEventResult);
//            Func<IDomainEvent, Task<DomainEventResult>> handlerFunc = null;
//            if (returnType == null)
//            {
//                var handlerAction = Delegate.CreateDelegate(typeof(Action<IDomainEvent>), handler, actionMember) as Action<IDomainEvent>;
//                handlerFunc = e =>
//                {
//                    handlerAction.Invoke(e);
//                    return Task.FromResult(DomainEventResult.EmptyResult());
//                };
//            }
//            else
//            {
//                if (returnType == eventResultType)
//                {
//                    var handlerAction = Delegate.CreateDelegate(typeof(Func<IDomainEvent, DomainEventResult>), handler, actionMember) as Func<IDomainEvent, DomainEventResult>;
//                    handlerFunc = e =>
//                    {
//                        handlerAction.Invoke(e);
//                        return Task.FromResult(DomainEventResult.EmptyResult());
//                    };
//                }
//                else if (returnType == typeof(Task<DataEventExecutionResult>))
//                {
//                    handlerFunc = Delegate.CreateDelegate(typeof(Func<IDomainEvent, Task<DomainEventResult>>), handler, actionMember) as Func<IDomainEvent, Task<DomainEventResult>>;
//                }
//            }

//            ExceptionUtil.ThrowFrameworkErrorIf(handlerFunc == null, $"{handlerActionName} not an event handling method ");

//            // configure options
//            var options = new DomainEventHandlerOptions();
//            configure?.Invoke(options);

//            return new DefaultDomainEventHandler<IDomainEvent>(handlerFunc, options);
//        }

//        #endregion

//        #region Trigger event

//        /// <summary>
//        /// Trigger domain event
//        /// </summary>
//        /// <param name="triggerTime">Trigger time</param>
//        /// <param name="domainEvents">Domain events</param>
//        internal async Task TriggerEventAsync(EventTriggerTime triggerTime, params IDomainEvent[] domainEvents)
//        {
//            IEnumerable<IDomainEvent> eventCollection = domainEvents;
//            await TriggerEventAsync(triggerTime, eventCollection).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Trigger domain event
//        /// </summary>
//        /// <param name="triggerTime">Trigger time</param>
//        /// <param name="domainEvents">Domain events</param>
//        internal async Task TriggerEventAsync(EventTriggerTime triggerTime, IEnumerable<IDomainEvent> domainEvents)
//        {
//            if (domainEvents.IsNullOrEmpty())
//            {
//                return;
//            }
//            foreach (var domainEvent in domainEvents)
//            {
//                if (domainEvent == null)
//                {
//                    continue;
//                }
//                //overall event handler
//                overallEventHandlers.TryGetValue(triggerTime, out var allHandlers);

//                //spectial event handler
//                var eventType = domainEvent.GetType();
//                List<IDomainEventHandler> tiggerTimeHandlers = null;
//                specialEventHandlers.TryGetValue(eventType.GUID, out var eventTypeHandlers);
//                eventTypeHandlers?.TryGetValue(triggerTime, out tiggerTimeHandlers);

//                // over all handler
//                if (!allHandlers.IsNullOrEmpty())
//                {
//                    foreach (var allHandler in allHandlers)
//                    {
//                        if (allHandler.Options?.Async ?? false)
//                        {
//                            _ = allHandler.ExecuteAsync(domainEvent);
//                        }
//                        else
//                        {
//                            await allHandler.ExecuteAsync(domainEvent).ConfigureAwait(false);
//                        }
//                    }
//                }

//                // special handler
//                if (!tiggerTimeHandlers.IsNullOrEmpty())
//                {
//                    foreach (var triggerTimeHandler in allHandlers)
//                    {
//                        if (triggerTimeHandler.Options?.Async ?? false)
//                        {
//                            _ = triggerTimeHandler.ExecuteAsync(domainEvent);
//                        }
//                        else
//                        {
//                            await triggerTimeHandler.ExecuteAsync(domainEvent).ConfigureAwait(false);
//                        }
//                    }
//                }
//            }
//        }

//        #endregion

//        #region Reset

//        /// <summary>
//        /// Clear all event handler
//        /// </summary>
//        public void Clear()
//        {
//            specialEventHandlers.Clear();
//            overallEventHandlers.Clear();
//        }

//        #endregion
//    }
//}
