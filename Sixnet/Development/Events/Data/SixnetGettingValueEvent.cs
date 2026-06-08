// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Getting value data event
    /// </summary>
    [Serializable]
    public class SixnetGettingValueEvent : SixnetBaseDataEvent
    {
        public SixnetGettingValueEvent()
        {
            EventType = SixnetDataEventType.GettingValue;
        }

        public static SixnetCheckingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetCheckingDataEvent>(dataClient, command);
        }
    }
}
