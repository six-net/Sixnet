using System;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Event;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Default data event handler
    /// </summary>
    public class DefaultDataEventHandler<TEvent> : DefaultEventHandler<TEvent>, ISixnetDataEventHandler where TEvent : ISixnetDataEvent
    {
        public DefaultDataEventHandler(Func<TEvent, CancellationToken, Task> handlerExecutor, DataEventHandlerOptions options = null) : base(handlerExecutor, options?.GetSixnetEventHandlerOptions())
        {
        }

        /// <summary>
        /// Handle data event
        /// </summary>
        /// <param name="dataEvent">Data event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public override Task Handle(ISixnetEvent dataEvent, CancellationToken cancellationToken)
        {
            return base.Handle((TEvent)dataEvent, cancellationToken);
        }
    }
}
