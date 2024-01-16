using System;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data.Event;
using Sixnet.Development.Events;
using Sixnet.Exceptions;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Default domain event handler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class DefaultDomainEventHandler<TEvent> : DefaultSixnetEventHandler<TEvent>, IDomainEventHandler where TEvent : IDomainEvent
    {

        public DefaultDomainEventHandler(Func<TEvent, CancellationToken, Task> handlerExecutor, DomainEventHandlerOptions options = null) : base(handlerExecutor, options?.GetSixnetEventHandlerOptions())
        {
        }

        /// <summary>
        /// Handle data event
        /// </summary>
        /// <param name="dataEvent">Data event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public override Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            return base.Handle(eventData, cancellationToken);
        }
    }
}
