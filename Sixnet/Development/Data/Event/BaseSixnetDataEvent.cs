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
    public class BaseSixnetDataEvent : ISixnetDataEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the created datetime
        /// </summary>
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;

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
        public ISixnetDataClient DataClient { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetDataCommand Command { get; set; }

        #endregion

        public static TEvent Create<TEvent>(ISixnetDataClient dataClient, SixnetDataCommand command) where TEvent : BaseSixnetDataEvent, new()
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            return new TEvent()
            {
                EntityType = command.GetEntityType(),
                Command = command,
                DataClient = dataClient
            };
        }
    }
}
