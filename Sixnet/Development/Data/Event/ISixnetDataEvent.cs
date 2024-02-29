using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Event;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data access event contract
    /// </summary>
    public interface ISixnetDataEvent : ISixnetEvent
    {
        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        DataEventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the data client
        /// </summary>
        ISixnetDataClient DataClient { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        SixnetDataCommand Command { get; set; }
    }
}
