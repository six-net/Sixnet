using System;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Dase domain event
    /// </summary>
    [Serializable]
    public abstract class BaseDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or set sthe event created date
        /// </summary>
        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    }
}
