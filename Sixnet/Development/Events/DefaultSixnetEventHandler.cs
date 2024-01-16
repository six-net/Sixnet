using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Default sixnet event handler
    /// </summary>
    public class DefaultSixnetEventHandler<TEvent> : ISixnetEventHandler where TEvent : ISixnetEvent
    {
        /// <summary>
        /// Gets or sets handler executor
        /// </summary>
        public Func<TEvent, CancellationToken, Task> HandlerExecutor { get; set; }

        /// <summary>
        /// Gets or sets the event handler options
        /// </summary>
        public SixnetEventHandlerOptions Options { get; set; }

        public DefaultSixnetEventHandler(Func<TEvent, CancellationToken, Task> handlerExecutor, SixnetEventHandlerOptions options = null)
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
            ThrowHelper.ThrowFrameworkErrorIf(HandlerExecutor == null, "Event handler excutor is null");
            ThrowHelper.ThrowFrameworkErrorIf(eventData is not TEvent, $"Event is not {typeof(TEvent).FullName}");

            var executorTask = HandlerExecutor((TEvent)eventData, cancellationToken);
            return (Options?.Async ?? false) ? Task.CompletedTask : executorTask;
        }
    }
}
