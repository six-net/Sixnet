using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Got value event
    /// </summary>
    public class GotValueEvent
        : BaseDataEvent
    {
        public GotValueEvent()
        {
            EventType = DataEventType.GotValue;
        }

        /// <summary>
        /// Gets or sets the queryable
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        public static GotValueEvent Create(IDataClient dataClient, DataCommand command,  dynamic value)
        {
            ThrowHelper.ThrowArgNullIf(command == null, nameof(command));

            return new GotValueEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
