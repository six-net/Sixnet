// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Querying data event
    /// </summary>
    [Serializable]
    public class SixnetQueryingDataEvent : SixnetBaseDataEvent
    {
        public SixnetQueryingDataEvent()
        {
            EventType = SixnetDataEventType.Querying;
        }

        public static SixnetQueryingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetQueryingDataEvent>(dataClient, command);
        }
    }
}
