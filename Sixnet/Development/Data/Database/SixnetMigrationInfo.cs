// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Migration info
    /// </summary>
    public class SixnetMigrationInfo
    {
        /// <summary>
        /// Whether not fixed length
        /// </summary>
        public bool NotFixedLength { get; set; }

        /// <summary>
        /// Gets or sets the new tables
        /// </summary>
        public List<SixnetNewTableInfo> NewTables { get; set; }

        /// <summary>
        /// Rename tables
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName> RenameTables { get; set; }

        /// <summary>
        /// Gets or sets the deletable table names
        /// </summary>
        public List<SixnetDatabaseObjectName> DeletableTableNames { get; set; }

        /// <summary>
        /// Gets or sets the new fields
        /// Key: table name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>> NewFields { get; set; }

        /// <summary>
        /// Gets or sets the updatable fields
        /// Key: table name
        /// Value => key: old field name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, Dictionary<string, SixnetDataField>> UpdatableFields { get; set; }

        /// <summary>
        /// Gets or sets the deletable fields
        /// Key: table name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>> DeletableFields { get; set; }
    }
}
