// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Create date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class CreateDateEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Create date
        /// </summary>
        [SixnetEntityField(Description = "Create date", Role = SixnetFieldRole.CreateDate)]
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Create user id
        /// </summary>
        [SixnetEntityField(Description = "Create user id", Role = SixnetFieldRole.CreateUserId)]
        public long CreateUserId { get; set; }

        /// <summary>
        /// Create user name
        /// </summary>
        [SixnetEntityField(Description = "Create user name", Role = SixnetFieldRole.CreateUserName, Length = 50)]
        public string CreateUserName { get; set; }

        /// <summary>
        /// Create user display name
        /// </summary>
        [SixnetEntityField(Description = "Create user display name", Role = SixnetFieldRole.CreateUserDisplayName, Length = 200)]
        public string CreateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Tenant create date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantCreateDateEntity<TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [SixnetEntityField(Description = "Tenant id", Role = SixnetFieldRole.Isolation)]
        public long TenantId { get; set; }
    }

    /// <summary>
    /// Update date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class UpdateDateEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Update date
        /// </summary>
        [SixnetEntityField(Description = "Update date", Role = SixnetFieldRole.UpdateDate)]
        public DateTimeOffset UpdateDate { get; set; }

        /// <summary>
        /// Update user id
        /// </summary>
        [SixnetEntityField(Description = "Update user id", Role = SixnetFieldRole.UpdateUserId)]
        public long UpdateUserId { get; set; }

        /// <summary>
        /// Update user name
        /// </summary>
        [SixnetEntityField(Description = "Update user name", Role = SixnetFieldRole.UpdateUserName, Length = 50)]
        public string UpdateUserName { get; set; }

        /// <summary>
        /// Update user display name
        /// </summary>
        [SixnetEntityField(Description = "Update user display name", Role = SixnetFieldRole.UpdateUserDisplayName, Length = 200)]
        public string UpdateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Tenant update date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantUpdateDateEntity<TEntity> : UpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [SixnetEntityField(Description = "Tenant id", Role = SixnetFieldRole.Isolation)]
        public long TenantId { get; set; }
    }

    /// <summary>
    /// Create & Update entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class CreateUpdateDateEntity<TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Update date
        /// </summary>
        [SixnetEntityField(Description = "Update date", Role = SixnetFieldRole.UpdateDate)]
        public DateTimeOffset UpdateDate { get; set; }

        /// <summary>
        /// Update user id
        /// </summary>
        [SixnetEntityField(Description = "Update user id", Role = SixnetFieldRole.UpdateUserId)]
        public long UpdateUserId { get; set; }

        /// <summary>
        /// Update user name
        /// </summary>
        [SixnetEntityField(Description = "Update user name", Role = SixnetFieldRole.UpdateUserName, Length = 50)]
        public string UpdateUserName { get; set; }

        /// <summary>
        /// Update display name
        /// </summary>
        [SixnetEntityField(Description = "Update display name", Role = SixnetFieldRole.UpdateUserDisplayName, Length = 200)]
        public string UpdateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Center & Create & Update entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantCreateUpdateDateEntity<TEntity> : CreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [SixnetEntityField(Description = "Tenant id", Role = SixnetFieldRole.Isolation)]
        public long TenantId { get; set; }
    }
}
