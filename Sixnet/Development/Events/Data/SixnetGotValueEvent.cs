// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Got value event
    /// </summary>
    public class SixnetGotValueEvent
        : SixnetBaseDataEvent
    {
        public SixnetGotValueEvent()
        {
            EventType = SixnetDataEventType.GotValue;
        }

        /// <summary>
        /// Gets or sets the queryable
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        public static SixnetGotValueEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command, dynamic value)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new SixnetGotValueEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
