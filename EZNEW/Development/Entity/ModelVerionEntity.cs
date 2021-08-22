using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Domain.Model;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model version entity
    /// </summary>
    public class ModelVerionEntity<TEntity>
        : ModelRecordEntity<TEntity>, IVersionEntity
        where TEntity : BaseEntity<TEntity>, IModel<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the version value
        /// </summary>
        [EntityField(Description = "Version", Role = FieldRole.Version)]
        public long Version { get; set; }
    }
}
