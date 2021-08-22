using System;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default data save domain event
    /// </summary>
    [Serializable]
    public class DefaultSaveDomainEvent<T> : BaseDomainEvent
    {
        /// <summary>
        /// Gets or sets the save object
        /// </summary>
        public T Object { get; set; }
    }
}
