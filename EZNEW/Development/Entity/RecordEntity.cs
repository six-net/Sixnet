using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines record entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>

    public abstract class RecordEntity<TEntity> : BaseEntity<TEntity>, ICreationDateTimeEntity, IUpdateDateTimeEntity where TEntity : BaseEntity<TEntity>, new()
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
