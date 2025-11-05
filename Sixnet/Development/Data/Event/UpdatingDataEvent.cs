// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Updating data event
    /// </summary>
    [Serializable]
    public class UpdatingDataEvent : BaseSixnetDataEvent
    {
        public UpdatingDataEvent()
        {
            EventType = DataEventType.Updating;
        }

        public static UpdatingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<UpdatingDataEvent>(dataClient, command);
        }
    }
}
