// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    #region Normal

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetIdEntity<TId, TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetIdCreateDateEntity<TId, TEntity> : SixnetCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetIdUpdateDateEntity<TId, TEntity> : SixnetUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetIdCreateUpdateDateEntity<TId, TEntity> : SixnetCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantIdEntity<TId, TEntity> : SixnetTenantEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantIdCreateDateEntity<TId, TEntity> : SixnetTenantCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantIdUpdateDateEntity<TId, TEntity> : SixnetTenantUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantIdCreateUpdateDateEntity<TId, TEntity> : SixnetTenantCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.GeneratedId)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Archiveable id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableIdEntity<TId, TEntity> : SixnetIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableIdCreateDateEntity<TId, TEntity> : SixnetIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableIdUpdateDateEntity<TId, TEntity> : SixnetIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create & update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableIdCreateUpdateDateEntity<TId, TEntity> : SixnetIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantIdEntity<TId, TEntity> : SixnetTenantIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantIdCreateDateEntity<TId, TEntity> : SixnetTenantIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantIdUpdateDateEntity<TId, TEntity> : SixnetTenantIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantIdCreateUpdateDateEntity<TId, TEntity> : SixnetTenantIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    #endregion

    #region Increment

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetAutoIdEntity<TId, TEntity> : SixnetBaseEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetAutoIdCreateDateEntity<TId, TEntity> : SixnetCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetAutoIdUpdateDateEntity<TId, TEntity> : SixnetUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetAutoIdCreateUpdateDateEntity<TId, TEntity> : SixnetCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Tenant id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantAutoIdEntity<TId, TEntity> : SixnetTenantEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantAutoIdCreateDateEntity<TId, TEntity> : SixnetTenantCreateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantAutoIdUpdateDateEntity<TId, TEntity> : SixnetTenantUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Id entity 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetTenantAutoIdCreateUpdateDateEntity<TId, TEntity> : SixnetTenantCreateUpdateDateEntity<TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey | SixnetFieldRole.Increment)]
        public TId Id { get; set; }
    }

    /// <summary>
    /// Archiveable id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableAutoIdEntity<TId, TEntity> : SixnetAutoIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableAutoIdCreateDateEntity<TId, TEntity> : SixnetAutoIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableAutoIdUpdateDateEntity<TId, TEntity> : SixnetAutoIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable id create & update date entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableAutoIdCreateUpdateDateEntity<TId, TEntity> : SixnetAutoIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantAutoIdEntity<TId, TEntity> : SixnetTenantAutoIdEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantAutoIdCreateDateEntity<TId, TEntity> : SixnetTenantAutoIdCreateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantAutoIdUpdateDateEntity<TId, TEntity> : SixnetTenantAutoIdUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Archiveable center id entity
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SixnetArchiveableTenantAutoIdCreateUpdateDateEntity<TId, TEntity> : SixnetTenantAutoIdCreateUpdateDateEntity<TId, TEntity> where TEntity : class, ISixnetEntity<TEntity>, new()
    {
        /// <summary>
        /// Archived
        /// </summary>
        [SixnetEntityField(Description = "Archived", Role = SixnetFieldRole.Archive)]
        public bool IsArchived { get; set; }
    }

    #endregion
}
