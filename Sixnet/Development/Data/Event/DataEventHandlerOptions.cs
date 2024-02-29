using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Event;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data event handler options
    /// </summary>
    public class DataEventHandlerOptions : TimeEventHandlerOptions
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
