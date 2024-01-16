using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Add data event
    /// </summary>
    public class AddingDataEvent : BaseDataEvent
    {
        public AddingDataEvent()
        {
            EventType = DataEventType.Adding;
        }

        public static AddingDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<AddingDataEvent>(dataClient, command);
        }
    }
}
