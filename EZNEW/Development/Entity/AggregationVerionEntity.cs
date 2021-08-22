using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Domain.Aggregation;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines aggregation version entity
    /// </summary>
    public class AggregationVerionEntity<TEntity>
        : AggregationRecordEntity<TEntity>, IVersionEntity
        where TEntity : BaseEntity<TEntity>, IAggregationRoot<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the version value
        /// </summary>
        [EntityField(Description = "Version", Role = FieldRole.Version)]
        public long Version { get; set; }
    }
}
