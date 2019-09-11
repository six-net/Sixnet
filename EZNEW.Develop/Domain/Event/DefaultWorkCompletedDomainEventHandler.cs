using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    public class DefaultWorkCompletedDomainEventHandler<Event> : DefaultDomainEventHandler<Event>, IDomainEventHandler where Event : class, IDomainEvent
    {
        /// <summary>
        /// execute time
        /// </summary>
        public EventTriggerTime ExecuteTime { get; } = EventTriggerTime.WorkCompleted;
    }
}
