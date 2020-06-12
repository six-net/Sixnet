using System;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// Default domain event handler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class DefaultDomainEventHandler<TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Gets or sets the execute event operation
        /// </summary>
        public Func<TEvent, DomainEventExecuteResult> ExecuteEventOperation
        {
            get; set;
        }

        /// <summary>
        /// Execute domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <returns>Return domain event execute result</returns>
        public DomainEventExecuteResult Execute(IDomainEvent domainEvent)
        {
            if (ExecuteEventOperation == null)
            {
                return DomainEventExecuteResult.EmptyResult("Did't set any event operation");
            }
            var eventData = domainEvent as TEvent;
            if (eventData == null)
            {
                return DomainEventExecuteResult.EmptyResult("Event data is null");
            }
            return ExecuteEventOperation(eventData);
        }
    }
}
