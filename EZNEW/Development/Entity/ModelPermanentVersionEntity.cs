using EZNEW.Development.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model permanent version entity
    /// </summary>
    public abstract class ModelPermanentVersionEntity<TEntity>
        : ModelVerionEntity<TEntity>, IObsoleteEntity
        where TEntity : BaseEntity<TEntity>, IModel<TEntity>, new()
    {
        /// <summary>
        /// Indecates whether is obsolete
        /// </summary>
        [EntityField(Description = "Obsolete", Role = FieldRole.ObsoleteTag)]
        public bool IsObsolete { get; set; }
    }
}
