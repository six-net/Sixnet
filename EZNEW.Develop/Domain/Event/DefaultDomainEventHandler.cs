using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    public class DefaultDomainEventHandler<Event> where Event : class, IDomainEvent
    {
        /// <summary>
        /// execute event operation
        /// </summary>
        public Func<Event, Task<DomainEventExecuteResult>> ExecuteEventOperation
        {
            get; set;
        }

        public DomainEventExecuteResult Execute(IDomainEvent domainEvent)
        {
            return ExecuteAsync(domainEvent).Result;
        }

        public async Task<DomainEventExecuteResult> ExecuteAsync(IDomainEvent domainEvent)
        {
            if (ExecuteEventOperation == null)
            {
                return DomainEventExecuteResult.EmptyResult("did't set any event operation");
            }
            var eventData = domainEvent as Event;
            if (eventData == null)
            {
                return DomainEventExecuteResult.EmptyResult("event data is null");
            }
            return await ExecuteEventOperation(eventData);
        }
    }
}
