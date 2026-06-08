// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Updated data event
    /// </summary>
    public class SixnetUpdatedDataEvent : SixnetBaseDataEvent
    {
        public SixnetUpdatedDataEvent()
        {
            EventType = SixnetDataEventType.Updated;
        }

        public static SixnetUpdatedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new SixnetUpdatedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
