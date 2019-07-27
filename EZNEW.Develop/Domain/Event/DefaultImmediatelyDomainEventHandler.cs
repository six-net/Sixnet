using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// default domain event handler
    /// </summary>
    public class DefaultImmediatelyDomainEventHandler<Event> : DefaultDomainEventHandler<Event>, IDomainEventHandler where Event : class, IDomainEvent
    {
        /// <summary>
        /// execute time
        /// </summary>
        public EventExecuteTime ExecuteTime { get; } = EventExecuteTime.Immediately;
    }
}
