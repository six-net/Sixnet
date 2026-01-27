// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Create date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class CreateDateEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Create date
        /// </summary>
        [EntityField(Description = "Create date", Role = FieldRole.CreateDate)]
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Create user id
        /// </summary>
        [EntityField(Description = "Create user id", Role = FieldRole.CreateUserId)]
        public long CreateUserId { get; set; }

        /// <summary>
        /// Create user name
        /// </summary>
        [EntityField(Description = "Create user name", Role = FieldRole.CreateUserName, Length = 50)]
        public string CreateUserName { get; set; }

        /// <summary>
        /// Create user display name
        /// </summary>
        [EntityField(Description = "Create user display name", Role = FieldRole.CreateUserDisplayName, Length = 200)]
        public string CreateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Tenant create date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantCreateDateEntity<TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [EntityField(Description = "Tenant id", Role = FieldRole.Isolation)]
        public long TenantId { get; set; }
    }

    /// <summary>
    /// Update date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class UpdateDateEntity<TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Update date
        /// </summary>
        [EntityField(Description = "Update date", Role = FieldRole.UpdateDate)]
        public DateTimeOffset UpdateDate { get; set; }

        /// <summary>
        /// Update user id
        /// </summary>
        [EntityField(Description = "Update user id", Role = FieldRole.UpdateUserId)]
        public long UpdateUserId { get; set; }

        /// <summary>
        /// Update user name
        /// </summary>
        [EntityField(Description = "Update user name", Role = FieldRole.UpdateUserName, Length = 50)]
        public string UpdateUserName { get; set; }

        /// <summary>
        /// Update user display name
        /// </summary>
        [EntityField(Description = "Update user display name", Role = FieldRole.UpdateUserDisplayName, Length = 200)]
        public string UpdateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Tenant update date entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantUpdateDateEntity<TEntity> : UpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [EntityField(Description = "Tenant id", Role = FieldRole.Isolation)]
        public long TenantId { get; set; }
    }

    /// <summary>
    /// Create & Update entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class CreateUpdateDateEntity<TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Update date
        /// </summary>
        [EntityField(Description = "Update date", Role = FieldRole.UpdateDate)]
        public DateTimeOffset UpdateDate { get; set; }

        /// <summary>
        /// Update user id
        /// </summary>
        [EntityField(Description = "Update user id", Role = FieldRole.UpdateUserId)]
        public long UpdateUserId { get; set; }

        /// <summary>
        /// Update user name
        /// </summary>
        [EntityField(Description = "Update user name", Role = FieldRole.UpdateUserName, Length = 50)]
        public string UpdateUserName { get; set; }

        /// <summary>
        /// Update display name
        /// </summary>
        [EntityField(Description = "Update display name", Role = FieldRole.UpdateUserDisplayName, Length = 200)]
        public string UpdateUserDisplayName { get; set; }
    }

    /// <summary>
    /// Center & Create & Update entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantCreateUpdateDateEntity<TEntity> : CreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        [EntityField(Description = "Tenant id", Role = FieldRole.Isolation)]
        public long TenantId { get; set; }
    }
}
