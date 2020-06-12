using System;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Data access event contract
    /// </summary>
    public interface IDataAccessEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the trigger date
        /// </summary>
        DateTimeOffset TriggerDate { get; set; }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        DataAccessEventType EventType { get; set; }
    }
}
