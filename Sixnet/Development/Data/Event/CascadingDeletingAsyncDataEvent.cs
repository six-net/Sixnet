using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Deleting async data event
    /// </summary>
    public class CascadingDeletingAsyncDataEvent : BaseSixnetDataEvent
    {
        public CascadingDeletingAsyncDataEvent()
        {
            EventType = DataEventType.Deleting;
        }

        public static CascadingDeletingAsyncDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<CascadingDeletingAsyncDataEvent>(dataClient, command);
        }
    }
}
