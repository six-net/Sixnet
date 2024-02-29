using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Add data event
    /// </summary>
    public class AddingDataEvent : BaseSixnetDataEvent
    {
        public AddingDataEvent()
        {
            EventType = DataEventType.Adding;
        }

        public static AddingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<AddingDataEvent>(dataClient, command);
        }
    }
}
