using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Checking data event
    /// </summary>
    [Serializable]
    public class CheckingDataEvent : BaseSixnetDataEvent
    {
        public CheckingDataEvent()
        {
            EventType = DataEventType.Checking;
        }

        public static CheckingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<CheckingDataEvent>(dataClient, command);
        }
    }
}
