using System;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Domain event contract
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the event created date
        /// </summary>
        DateTimeOffset CreatedDate { get; set; }
    }
}
