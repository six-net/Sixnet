// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Added data event
    /// </summary>
    public class SixnetAddedDataEvent : SixnetBaseDataEvent
    {
        public SixnetAddedDataEvent()
        {
            EventType = SixnetDataEventType.Added;
        }

        public static SixnetAddedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new SixnetAddedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
