// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Deleting async data event
    /// </summary>
    public class SixnetCascadingDeletingAsyncDataEvent : SixnetBaseDataEvent
    {
        public SixnetCascadingDeletingAsyncDataEvent()
        {
            EventType = SixnetDataEventType.Deleting;
        }

        public static SixnetCascadingDeletingAsyncDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetCascadingDeletingAsyncDataEvent>(dataClient, command);
        }
    }
}
