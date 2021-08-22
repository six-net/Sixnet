using EZNEW.Development.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Domain.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelMetadataAttribute : Attribute
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
