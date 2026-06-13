// "Company © 2025. All rights reserved."

using Sixnet.Development.Events;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Sixnet base event
    /// </summary>
    [Serializable]
    public abstract class SixnetBaseEvent : ISixnetEvent
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
