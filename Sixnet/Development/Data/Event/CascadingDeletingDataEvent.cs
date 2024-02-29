using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Deleting event
    /// </summary>
    [Serializable]
    internal class CascadingDeletingDataEvent : BaseSixnetDataEvent
    {
        public CascadingDeletingDataEvent()
        {
            EventType = DataEventType.Deleting;
        }

        public static CascadingDeletingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<CascadingDeletingDataEvent>(dataClient, command);
        }
    }
}
