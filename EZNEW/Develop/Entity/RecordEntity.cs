using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines record entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>

    public abstract class RecordEntity<TEntity> : DateTimeEntity<TEntity>, IObsoleteEntity where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Indecates whether is obsolete
        /// </summary>
        [EntityField(Description = "Obsolete", Role = FieldRole.ObsoleteTag)]
        public bool IsObsolete { get; set; }
    }
}
