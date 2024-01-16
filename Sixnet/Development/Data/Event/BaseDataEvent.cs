using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Base data event
    /// </summary>
    [Serializable]
    public class BaseDataEvent : IDataEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the created datetime
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public DataEventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the data client
        /// </summary>
        public IDataClient DataClient { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public DataCommand Command { get; set; }

        #endregion

        public static TEvent Create<TEvent>(IDataClient dataClient, DataCommand command) where TEvent : BaseDataEvent, new()
        {
            ThrowHelper.ThrowArgNullIf(command == null, nameof(command));

            return new TEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
