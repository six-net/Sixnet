using System;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Data access event base
    /// </summary>
    [Serializable]
    public class BaseDataAccessEvent : IDataAccessEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public DateTimeOffset TriggerDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public DataAccessEventType EventType { get; set; }

        #endregion
    }
}
