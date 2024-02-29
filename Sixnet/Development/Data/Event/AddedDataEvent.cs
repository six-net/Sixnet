using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Added data event
    /// </summary>
    public class AddedDataEvent : BaseSixnetDataEvent
    {
        public AddedDataEvent()
        {
            EventType = DataEventType.Added;
        }

        public static AddedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new AddedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
