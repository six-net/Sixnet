using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Querying data event
    /// </summary>
    [Serializable]
    public class QueryingDataEvent : BaseDataEvent
    {
        public QueryingDataEvent()
        {
            EventType = DataEventType.Querying;
        }

        public static QueryingDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<QueryingDataEvent>(dataClient, command);
        }
    }
}
