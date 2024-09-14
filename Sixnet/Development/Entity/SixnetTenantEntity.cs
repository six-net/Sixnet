using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Tenant entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TenantEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [EntityField(Description = "Tenant id", Role = FieldRole.Isolation)]
        public long TenantId { get; set; }
    }
}
