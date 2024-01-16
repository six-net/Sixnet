using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Getting value data event
    /// </summary>
    [Serializable]
    public class GettingValueEvent : BaseDataEvent
    {
        public GettingValueEvent()
        {
            EventType = DataEventType.GettingValue;
        }

        public static CheckingDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<CheckingDataEvent>(dataClient, command);
        }
    }
}
