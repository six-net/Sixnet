// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Getting value data event
    /// </summary>
    [Serializable]
    public class GettingValueEvent : BaseSixnetDataEvent
    {
        public GettingValueEvent()
        {
            EventType = DataEventType.GettingValue;
        }

        public static CheckingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<CheckingDataEvent>(dataClient, command);
        }
    }
}
