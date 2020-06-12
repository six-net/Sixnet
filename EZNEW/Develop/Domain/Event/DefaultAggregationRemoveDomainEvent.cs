using System;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// Default aggregation data remove domain event
    /// </summary>
    [Serializable]
    public class DefaultAggregationRemoveDomainEvent<T> : BaseDomainEvent
    {
        /// <summary>
        /// Gets or sets the remove object
        /// </summary>
        public T Object
        {
            get; set;
        }
    }
}
