﻿using System;
using System.Collections.Generic;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Entity config
    /// </summary>
    public class EntityConfiguration
    {
        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the entity group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the all fields
        /// </summary>
        public Dictionary<string, EntityField> AllFields { get; set; }

        /// <summary>
        /// Gets or sets the query fields
        /// </summary>
        public List<string> QueryFields { get; set; }

        /// <summary>
        /// Gets or sets the query entity fields
        /// </summary>
        internal List<EntityField> QueryEntityFields { get; set; }

        /// <summary>
        /// Gets or sets the must query fields
        /// </summary>
        public List<string> MustQueryFields { get; set; }

        /// <summary>
        /// Gets or sets the edit fields
        /// </summary>
        public List<EntityField> EditFields { get; set; }

        /// <summary>
        /// Gets or sets the primary keys
        /// </summary>
        public List<string> PrimaryKeys { get; set; }

        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<string> CacheKeys { get; set; }

        /// <summary>
        /// Gets or sets the cache prefix keys
        /// </summary>
        public List<string> CachePrefixKeys { get; set; }

        /// <summary>
        /// Gets or sets the cache ignore keys
        /// </summary>
        public List<string> CacheIgnoreKeys { get; set; }

        /// <summary>
        /// Gets or sets the version field name
        /// </summary>
        public string VersionField { get; set; }

        /// <summary>
        /// Gets or sets the update datetime field name
        /// </summary>
        public string UpdateDateTimeField { get; set; }

        /// <summary>
        /// Gets or sets the creation datatime field name
        /// </summary>
        public string CreationDateTimeField { get; set; }

        /// <summary>
        /// Gets or sets the obsolete field
        /// </summary>
        public string ObsoleteField { get; set; }

        /// <summary>
        /// Gets or sets the relation fields
        /// key:relation type id
        /// value: key->field,value:relation field
        /// </summary>
        public Dictionary<Guid, Dictionary<string, string>> RelationFields { get; set; }

        /// <summary>
        /// Gets or sets the predicate type
        /// </summary>
        public Type PredicateType { get; set; }

        /// <summary>
        /// Gets or sets the entity structure
        /// </summary>
        public EntityStructure Structure { get; set; } = EntityStructure.Normal;

        /// <summary>
        /// Indecates whether enable data cache
        /// </summary>
        public bool EnableCache { get; set; } = false;
    }
}
