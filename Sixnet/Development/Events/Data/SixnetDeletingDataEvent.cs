// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Deleting event
    /// </summary>
    [Serializable]
    public class SixnetDeletingDataEvent : SixnetBaseDataEvent
    {
        public SixnetDeletingDataEvent()
        {
            EventType = SixnetDataEventType.Deleting;
        }

        public static SixnetDeletingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetDeletingDataEvent>(dataClient, command);
        }
    }
}
