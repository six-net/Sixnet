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
    internal class CascadingDeletingAsyncDataEvent : BaseDataEvent
    {
        public CascadingDeletingAsyncDataEvent()
        {
            EventType = DataEventType.Deleting;
        }

        public static CascadingDeletingAsyncDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<CascadingDeletingAsyncDataEvent>(dataClient, command);
        }
    }
}
