using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Events;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data event handler options
    /// </summary>
    public class DataEventHandlerOptions : TimeSixnetEventHandlerOptions
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
