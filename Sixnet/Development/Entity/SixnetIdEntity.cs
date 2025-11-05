// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    #region Normal

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class IdEntity<TId, TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class IdCreateDateEntity<TId, TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class IdUpdateDateEntity<TId, TEntity> : UpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class IdCreateUpdateDateEntity<TId, TEntity> : CreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantIdEntity<TId, TEntity> : TenantEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantIdCreateDateEntity<TId, TEntity> : TenantCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantIdUpdateDateEntity<TId, TEntity> : TenantUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantIdCreateUpdateDateEntity<TId, TEntity> : TenantCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Archiveable id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableIdEntity<TId, TEntity> : IdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableIdCreateDateEntity<TId, TEntity> : IdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableIdUpdateDateEntity<TId, TEntity> : IdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create & update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableIdCreateUpdateDateEntity<TId, TEntity> : IdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantIdEntity<TId, TEntity> : TenantIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantIdCreateDateEntity<TId, TEntity> : TenantIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantIdUpdateDateEntity<TId, TEntity> : TenantIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantIdCreateUpdateDateEntity<TId, TEntity> : TenantIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    #endregion

    #region Increment

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AutoIdEntity<TId, TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AutoIdCreateDateEntity<TId, TEntity> : CreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AutoIdUpdateDateEntity<TId, TEntity> : UpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AutoIdCreateUpdateDateEntity<TId, TEntity> : CreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantAutoIdEntity<TId, TEntity> : TenantEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantAutoIdCreateDateEntity<TId, TEntity> : TenantCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantAutoIdUpdateDateEntity<TId, TEntity> : TenantUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TenantAutoIdCreateUpdateDateEntity<TId, TEntity> : TenantCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey | FieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Archiveable id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableAutoIdEntity<TId, TEntity> : AutoIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableAutoIdCreateDateEntity<TId, TEntity> : AutoIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableAutoIdUpdateDateEntity<TId, TEntity> : AutoIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create & update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableAutoIdCreateUpdateDateEntity<TId, TEntity> : AutoIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantAutoIdEntity<TId, TEntity> : TenantAutoIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantAutoIdCreateDateEntity<TId, TEntity> : TenantAutoIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantAutoIdUpdateDateEntity<TId, TEntity> : TenantAutoIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ArchiveableTenantAutoIdCreateUpdateDateEntity<TId, TEntity> : TenantAutoIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>
    {
        /// <summary>
        /// Archived
        /// </summary>
        [EntityField(Description = "Archived", Role = FieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    #endregion
}
