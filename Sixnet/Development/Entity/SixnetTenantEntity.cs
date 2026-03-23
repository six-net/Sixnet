// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Tenant entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TenantEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [EntityField(Description = "Tenant id", Role = FieldRole.Isolation)]
        public long TenantId { get; set; }
    }
}
