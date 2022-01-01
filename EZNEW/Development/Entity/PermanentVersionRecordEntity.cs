using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines permanent version record entity
    /// </summary>
    public class PermanentVersionRecordEntity<TEntity> 
        : VersionEntity<TEntity>, IObsoleteEntity 
        where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Indicates whether is obsolete
        /// </summary>
        [EntityField(Description = "Obsolete", Role = FieldRole.ObsoleteTag)]
        public bool IsObsolete { get; set; }
    }
}
