using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines datetime entity
    /// </summary>
    public abstract class DateTimeEntity<TEntity> : BaseEntity<TEntity>, ICreationDateTimeEntity, IRefreshDateTimeEntity where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the creation datetime
        /// </summary>
        [EntityField(Description ="Creation Datetime",Role =FieldRole.CreationDateTime)]
        public DateTimeOffset CreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the refresh datetime
        /// </summary>
        [EntityField(Description = "Refresh Datetime", Role = FieldRole.RefreshDateTime)]
        public DateTimeOffset RefreshDateTime { get; set; }
    }
}
