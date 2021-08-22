using System;
using System.Threading.Tasks;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default domain event handler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class DefaultDomainEventHandler<TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event execution operation
        /// </summary>
        public Func<TEvent, DomainEventResult> EventExecutionOperation { get; set; }

        /// <summary>
        /// Execute domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <returns>Return domain event execute result</returns>
        public DomainEventResult Execute(IDomainEvent domainEvent)
        {
            if (EventExecutionOperation == null)
            {
                return DomainEventResult.EmptyResult("Did't set any event execution operation");
            }
            TEvent eventData = domainEvent as TEvent;
            if (eventData == null)
            {
                return DomainEventResult.EmptyResult("Event data is null");
            }
            return EventExecutionOperation(eventData);
        }
    }
}
