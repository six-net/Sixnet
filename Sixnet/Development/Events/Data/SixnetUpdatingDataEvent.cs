// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Updating data event
    /// </summary>
    [Serializable]
    public class SixnetUpdatingDataEvent : SixnetBaseDataEvent
    {
        public SixnetUpdatingDataEvent()
        {
            EventType = SixnetDataEventType.Updating;
        }

        public static SixnetUpdatingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetUpdatingDataEvent>(dataClient, command);
        }
    }
}
