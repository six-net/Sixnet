// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Deleting event
    /// </summary>
    [Serializable]
    internal class SixnetCascadingDeletingDataEvent : SixnetBaseDataEvent
    {
        public SixnetCascadingDeletingDataEvent()
        {
            EventType = SixnetDataEventType.Deleting;
        }

        public static SixnetCascadingDeletingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetCascadingDeletingDataEvent>(dataClient, command);
        }
    }
}
