// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Deleting event
    /// </summary>
    [Serializable]
    public class DeletingDataEvent : BaseSixnetDataEvent
    {
        public DeletingDataEvent()
        {
            EventType = DataEventType.Deleting;
        }

        public static DeletingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<DeletingDataEvent>(dataClient, command);
        }
    }
}
