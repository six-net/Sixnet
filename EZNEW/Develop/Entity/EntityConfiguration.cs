using System;
using System.Collections.Generic;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity config
    /// </summary>
    public class EntityConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets all fields
        /// </summary>
        public List<EntityField> AllFields
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets query fields
        /// </summary>
        public List<EntityField> QueryFields
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets primary keys
        /// </summary>
        public List<EntityField> PrimaryKeys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets cache keys
        /// </summary>
        public List<EntityField> CacheKeys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets cache prefix keys
        /// </summary>
        public List<EntityField> CachePrefixKeys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets cache ignore keys
        /// </summary>
        public List<EntityField> CacheIgnoreKeys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the version key
        /// </summary>
        public EntityField VersionField
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the refresh date key
        /// </summary>
        public EntityField RefreshDateField
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the relation fields
        /// key:relation type id
        /// value: key->field,value:relation field
        /// </summary>
        public Dictionary<Guid, Dictionary<string, string>> RelationFields
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the predicate type
        /// </summary>
        public Type PredicateType
        {
            get; set;
        }

        #endregion
    }
}
