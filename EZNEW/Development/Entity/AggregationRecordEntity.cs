using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Domain.Aggregation;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines aggregation record entity
    /// </summary>
    public class AggregationRecordEntity<TEntity>
        : AggregationEntity<TEntity>, ICreationDateTimeEntity, IUpdateDateTimeEntity
        where TEntity : BaseEntity<TEntity>, IAggregationRoot<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the creation datetime
        /// </summary>
        [EntityField(Description = "CreationDatetime", Role = FieldRole.CreationDateTime)]
        public DateTimeOffset CreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the update datetime
        /// </summary>
        [EntityField(Description = "UpdateDatetime", Role = FieldRole.UpdateDateTime)]
        public DateTimeOffset UpdateDateTime { get; set; }
    }
}
