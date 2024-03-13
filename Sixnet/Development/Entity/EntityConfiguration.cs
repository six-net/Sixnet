using System;
using System.Collections.Generic;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Entity
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
        /// Gets or sets the entity group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the comment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the all fields
        /// </summary>
        public Dictionary<string, DataField> AllFields { get; set; }

        /// <summary>
        /// Gets or sets the queryable fields
        /// </summary>
        internal List<DataField> QueryableFields { get; set; }

        /// <summary>
        /// Gets or sets the necessary fields
        /// </summary>
        internal List<DataField> NecessaryQueryableFields { get; set; }

        /// <summary>
        /// Gets or sets the edit fields
        /// </summary>
        public List<DataField> EditableFields { get; set; }

        /// <summary>
        /// Gets or sets the cache field names
        /// </summary>
        public List<string> CacheFieldNames { get; set; }

        /// <summary>
        /// Gets or sets the cache prefix field names
        /// </summary>
        public List<string> CachePrefixFieldNames { get; set; }

        /// <summary>
        /// Gets or sets the cache ignore field names
        /// </summary>
        public List<string> CacheIgnoreFieldNames { get; set; }

        /// <summary>
        /// Gets or sets the relation fields
        /// Key:relation entity type id
        /// Value: key->field,value->relation options
        /// </summary>
        public Dictionary<Guid, Dictionary<string, EntityRelationFieldAttribute>> RelationFields { get; set; }

        /// <summary>
        /// Gets the role field names
        /// Key: field role
        /// Value: fields
        /// </summary>
        public Dictionary<FieldRole, List<DataField>> RoleFields { get; set; }

        /// <summary>
        /// Gets or sets the predicate type
        /// </summary>
        public Type PredicateType { get; set; }

        /// <summary>
        /// Indicates whether enable data cache
        /// </summary>
        public bool EnableCache { get; set; } = false;

        /// <summary>
        /// Gets or sets the entity style
        /// </summary>
        public EntityStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the split table type
        /// </summary>
        public SplitTableType SplitTableType { get; set; }

        /// <summary>
        /// Gets or sets the split table provider name
        /// </summary>
        public string SplitTableProviderName { get; set; }

        /// <summary>
        /// Whether is split table
        /// </summary>
        public bool IsSplitTable => SplitTableType != SplitTableType.None;
    }
}
