using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Events;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Domain event handler options
    /// </summary>
    public class DomainEventHandlerOptions : TimeSixnetEventHandlerOptions
    {
        public SixnetEventHandlerOptions GetSixnetEventHandlerOptions()
        {
            return new SixnetEventHandlerOptions()
            {
                Async = Async
            };
        }
    }
}
