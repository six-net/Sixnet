using System;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// Default aggregation data save domain event
    /// </summary>
    [Serializable]
    public class DefaultAggregationSaveDomainEvent<T> : BaseDomainEvent
    {
        /// <summary>
        /// Gets or sets the save object
        /// </summary>
        public T Object
        {
            get; set;
        }
    }
}
