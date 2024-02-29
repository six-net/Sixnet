using System;
using System.Collections.Generic;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Migration info
    /// </summary>
    public class MigrationInfo
    {
        /// <summary>
        /// Whether not fixed length
        /// </summary>
        public bool NotFixedLength { get; set; }

        /// <summary>
        /// Gets or sets the new tables
        /// </summary>
        public List<NewTableInfo> NewTables { get; set; }

        /// <summary>
        /// Gets or sets the deletable table names
        /// </summary>
        public List<string> DeletableTableNames { get; set; }

        /// <summary>
        /// Gets or sets the new fields
        /// Key: entity type
        /// </summary>
        public Dictionary<Type, EntityField> NewFields { get; set; }

        /// <summary>
        /// Gets or sets the updatable fields
        /// Key: entity type
        /// </summary>
        public Dictionary<Type, EntityField> UpdatableFields { get; set; }

        /// <summary>
        /// Gets or sets the deletable fields
        /// Key: entity type
        /// </summary>
        public Dictionary<Type, EntityField> DeletableFields { get; set; }
    }
}
