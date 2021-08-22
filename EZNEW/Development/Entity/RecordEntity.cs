using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines record entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>

    public abstract class RecordEntity<TEntity> : BaseEntity<TEntity>, ICreationTimeEntity, IUpdateTimeEntity where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        [EntityField(Description = "CreationTime", Role = FieldRole.CreationTime)]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the update time
        /// </summary>
        [EntityField(Description = "UpdateTime", Role = FieldRole.UpdateTime)]
        public DateTimeOffset UpdateTime { get; set; }
    }
}
