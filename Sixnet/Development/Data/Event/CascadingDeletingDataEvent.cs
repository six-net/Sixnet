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
    internal class CascadingDeletingDataEvent : BaseDataEvent
    {
        public CascadingDeletingDataEvent()
        {
            EventType = DataEventType.Deleting;
        }

        public static CascadingDeletingDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<CascadingDeletingDataEvent>(dataClient, command);
        }
    }
}
