// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;

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
        /// Renamed tables
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName> RenamedTables { get; set; }

        /// <summary>
        /// Gets or sets the deleted table names
        /// </summary>
        public List<SixnetDatabaseObjectName> DeletedTables { get; set; }

        /// <summary>
        /// Gets or sets the new fields
        /// Key: table name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>> NewFields { get; set; }

        /// <summary>
        /// Gets or sets the updated fields
        /// Key: table name
        /// Value => key: old field name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, Dictionary<string, SixnetDataField>> UpdatedFields { get; set; }

        /// <summary>
        /// Gets or sets the deleted fields
        /// Key: table name
        /// </summary>
        public Dictionary<SixnetDatabaseObjectName, List<SixnetDataField>> DeletedFields { get; set; }

        /// <summary>
        /// Gets or sets the new foreign keys
        /// Key: table name
        /// </summary>
        /// 
        public List<SixnetEntityForeignKeyInfo> NewForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets the deleted foreign keys
        /// </summary>
        public List<SixnetEntityForeignKeyInfo> DeletedForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets the new indexes
        /// </summary>
        public List<SixnetEntityIndexInfo> NewIndexes { get; set; }

        /// <summary>
        /// Gets or sets the deleted indexes
        /// </summary>
        public List<SixnetEntityIndexInfo> DeletedIndexes { get; set; }

        /// <summary>
        /// Whether delete all foreign keys
        /// </summary>
        public bool DeleteAllForeignKey { get; set; }

        /// <summary>
        /// Whether delete all tables
        /// </summary>
        public bool DeleteAllTable { get; set; }

        /// <summary>
        /// Whether delete all views
        /// </summary>
        public bool DeleteAllView { get; set; }

        /// <summary>
        /// Whether delete all procedure
        /// </summary>
        public bool DeleteAllProcedure { get; set; }

        /// <summary>
        /// Whether delete all function
        /// </summary>
        public bool DeleteAllFunction { get; set; }

        /// <summary>
        /// Whether delete all custom type
        /// </summary>
        public bool DeleteAllCustomType { get; set; }

        /// <summary>
        /// Whether clear database
        /// </summary>
        public bool ClearDatabase { get; set; }
    }
}
