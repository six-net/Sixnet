using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Querying data event
    /// </summary>
    [Serializable]
    public class QueryingDataEvent : BaseSixnetDataEvent
    {
        public QueryingDataEvent()
        {
            EventType = DataEventType.Querying;
        }

        public static QueryingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<QueryingDataEvent>(dataClient, command);
        }
    }
}
