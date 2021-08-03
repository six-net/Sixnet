using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines version entity
    /// </summary>
    public abstract class VersionEntity<TEntity> : DateTimeEntity<TEntity>, IVersionEntity where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the version value
        /// </summary>
        [EntityField(Description = "Version", Role = FieldRole.Version)]
        public long Version { get; set; }
    }
}
