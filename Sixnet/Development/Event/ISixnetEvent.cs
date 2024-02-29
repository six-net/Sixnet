using System;

namespace Sixnet.Development.Event
{
    /// <summary>
    /// Defines event contract
    /// </summary>
    public interface ISixnetEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        DateTimeOffset CreateDate { get; set; }
    }
}
