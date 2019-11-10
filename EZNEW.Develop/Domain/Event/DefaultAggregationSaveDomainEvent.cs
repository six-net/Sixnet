using EZNEW.Develop.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// default aggregation domain save event
    /// </summary>
    public class DefaultAggregationSaveDomainEvent<T> : BaseDomainEvent
    {
        /// <summary>
        /// save object
        /// </summary>
        public T Object
        {
            get; set;
        }
    }
}
