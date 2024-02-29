using System;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Exceptions;

namespace Sixnet.Development.Event
{
    /// <summary>
    /// Default sixnet event handler
    /// </summary>
    public class DefaultEventHandler<TEvent> : ISixnetEventHandler where TEvent : ISixnetEvent
    {
        /// <summary>
        /// Gets or sets handler executor
        /// </summary>
        public Func<TEvent, CancellationToken, Task> HandlerExecutor { get; set; }

        /// <summary>
        /// Gets or sets the event handler options
        /// </summary>
        public EventHandlerOptions Options { get; set; }

        public DefaultEventHandler(Func<TEvent, CancellationToken, Task> handlerExecutor, EventHandlerOptions options = null)
        {
            HandlerExecutor = handlerExecutor;
            Options = options;
        }

        /// <summary>
        /// Handle data event
        /// </summary>
        /// <param name="dataEvent">Data event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public virtual Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(HandlerExecutor == null, "Event handler excutor is null");
            SixnetDirectThrower.ThrowSixnetExceptionIf(eventData is not TEvent, $"Event is not {typeof(TEvent).FullName}");

            var executorTask = HandlerExecutor((TEvent)eventData, cancellationToken);
            return (Options?.Async ?? false) ? Task.CompletedTask : executorTask;
        }
    }
}
