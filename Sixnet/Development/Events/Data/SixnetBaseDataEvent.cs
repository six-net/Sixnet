// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Events;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Base data event
    /// </summary>
    [Serializable]
    public class SixnetBaseDataEvent : SixnetBaseEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public SixnetDataEventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the data client
        /// </summary>
        public ISixnetDataClient DataClient { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetDataCommand Command { get; set; }

        #endregion

        public static TEvent Create<TEvent>(ISixnetDataClient dataClient, SixnetDataCommand command) where TEvent : SixnetBaseDataEvent, new()
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
