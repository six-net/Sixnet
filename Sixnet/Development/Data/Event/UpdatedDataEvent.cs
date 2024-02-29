using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Updated data event
    /// </summary>
    public class UpdatedDataEvent : BaseSixnetDataEvent
    {
        public UpdatedDataEvent()
        {
            EventType = DataEventType.Updated;
        }

        public static UpdatedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new UpdatedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
