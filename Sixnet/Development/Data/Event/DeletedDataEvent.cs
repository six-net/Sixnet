using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Deleted data event
    /// </summary>
    public class DeletedDataEvent : BaseDataEvent
    {
        public DeletedDataEvent()
        {
            EventType = DataEventType.Deleted;
        }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets or sets the affected datas
        /// </summary>
        public int AffectedDatas { get; set; }

        public static DeletedDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            ThrowHelper.ThrowArgNullIf(command == null, nameof(command));

            return new DeletedDataEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }

    }
}
