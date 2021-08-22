using EZNEW.Development.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines aggregation permanent record
    /// </summary>
    public class AggregationPermanentRecordEntity<TEntity>
        : AggregationRecordEntity<TEntity>, IObsoleteEntity
        where TEntity : BaseEntity<TEntity>, IAggregationRoot<TEntity>, new()
    {
        /// <summary>
        /// Indecates whether is obsolete
        /// </summary>
        [EntityField(Description = "Obsolete", Role = FieldRole.ObsoleteTag)]
        public bool IsObsolete { get; set; }
    }
}
