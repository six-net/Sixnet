// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

using Sixnet.Development.Work;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Default sixnet event handler
    /// </summary>
    public class SixnetDefaultEventHandler : ISixnetEventHandler
    {
        /// <summary>
        /// Gets or sets handler executor
        /// </summary>
        public Func<object, CancellationToken, Task> HandlerExecutor { get; set; }

        /// <summary>
        /// Gets or sets the event handler options
        /// </summary>
        public SixnetEventHandlerOptions Options { get; set; }

        private SixnetDefaultEventHandler(Func<object, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            HandlerExecutor = handlerExecutor;
            var options = new SixnetEventHandlerOptions();
            configure?.Invoke(options);
            Options = options;
        }

        public static SixnetDefaultEventHandler GetDefaultEventHandler(Func<object, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            return new SixnetDefaultEventHandler(handlerExecutor, configure);
        }

        public static SixnetDefaultEventHandler GetDefaultEventHandler(Func<ISixnetEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null)
        {
            Func<object, CancellationToken, Task> objectExecutor = (e, ct) =>
            {
                return handlerExecutor(e as ISixnetEvent, ct);
            };
            return GetDefaultEventHandler(objectExecutor, configure);
        }

        public static SixnetDefaultEventHandler GetDefaultEventHandler<TEvent>(Func<TEvent, CancellationToken, Task> handlerExecutor, Action<SixnetEventHandlerOptions> configure = null) where TEvent : class
        {
            Func<object, CancellationToken, Task> objectExecutor = (e, ct) =>
            {
                return handlerExecutor(e as TEvent, ct);
            };
            return GetDefaultEventHandler(objectExecutor, configure);
        }

        /// <summary>
        /// Handle data event
        /// </summary>
        /// <param name="eventData">Data event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public virtual Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(HandlerExecutor == null, "Event handler excutor is null");

            var isAsync = Options?.Async ?? false;
            if (isAsync)
            {
                ThreadPool.QueueUserWorkItem(s =>
                {
                    SixnetUnitOfWork.Current = null;
                    HandlerExecutor(eventData, cancellationToken);
                });
                return Task.CompletedTask;
            }
            else
            {
                return HandlerExecutor(eventData, cancellationToken);
            }
        }
    }
}
