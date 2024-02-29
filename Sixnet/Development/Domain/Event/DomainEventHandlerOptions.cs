using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Event;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Domain event handler options
    /// </summary>
    public class DomainEventHandlerOptions : TimeEventHandlerOptions
    {
        public EventHandlerOptions GetSixnetEventHandlerOptions()
        {
            return new EventHandlerOptions()
            {
                Async = Async
            };
        }
    }
}
