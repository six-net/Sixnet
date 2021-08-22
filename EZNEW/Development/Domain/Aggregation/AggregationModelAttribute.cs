using EZNEW.Development.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Domain.Aggregation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AggregationModelAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the feature
        /// </summary>
        public AggregationFeature Feature { get; set; } = AggregationFeature.Default;

        /// <summary>
        /// Gets or sets the relation type
        /// </summary>
        public Type EntityType { get; set; }
    }
}
