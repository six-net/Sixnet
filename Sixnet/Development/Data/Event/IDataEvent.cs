using System;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Events;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Data access event contract
    /// </summary>
    public interface IDataEvent : ISixnetEvent
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
        IDataClient DataClient { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        DataCommand Command { get; set; }
    }
}
