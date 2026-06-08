// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Add data event
    /// </summary>
    public class SixnetAddingDataEvent : SixnetBaseDataEvent
    {
        public SixnetAddingDataEvent()
        {
            EventType = SixnetDataEventType.Adding;
        }

        public static SixnetAddingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetAddingDataEvent>(dataClient, command);
        }
    }
}
