// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Deleted data event
    /// </summary>
    public class SixnetDeletedDataEvent : SixnetBaseDataEvent
    {
        public SixnetDeletedDataEvent()
        {
            EventType = SixnetDataEventType.Deleted;
        }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets or sets the affected datas
        /// </summary>
        public int AffectedDatas { get; set; }

        public static SixnetDeletedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new SixnetDeletedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }

    }
}
