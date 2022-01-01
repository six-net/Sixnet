using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Domain.Model;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model record entity
    /// </summary>
    public abstract class ModelRecordEntity<TEntity>
        : ModelEntity<TEntity>, ICreationTimeEntity, IUpdateTimeEntity
        where TEntity : BaseEntity<TEntity>, IModel<TEntity>, new()
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
