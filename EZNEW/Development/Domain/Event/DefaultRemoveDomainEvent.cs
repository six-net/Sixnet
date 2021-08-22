using System;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default data remove domain event
    /// </summary>
    [Serializable]
    public class DefaultRemoveDomainEvent<T> : BaseDomainEvent
    {
        /// <summary>
        /// Gets or sets the remove object
        /// </summary>
        public T Object { get; set; }
    }
}
