using System;

namespace Sixnet.Development.Domain.Event
{
    /// <summary>
    /// Dase domain event
    /// </summary>
    [Serializable]
    public abstract class BaseDomainEvent : ISixnetDomainEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or set sthe event create date
        /// </summary>
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
    }
}
