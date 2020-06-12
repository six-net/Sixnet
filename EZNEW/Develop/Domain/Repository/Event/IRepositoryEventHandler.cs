using System;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Repository event handler contract
    /// </summary>
    public interface IRepositoryEventHandler
    {
        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the handler repository type
        /// </summary>
        Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        Type ObjectType { get; set; }
    }
}
