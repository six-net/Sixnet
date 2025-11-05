// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

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
        /// Rename tables
        /// </summary>
        public Dictionary<string, string> RenameTables { get; set; }

        /// <summary>
        /// Gets or sets the deletable table names
        /// </summary>
        public List<string> DeletableTableNames { get; set; }

        /// <summary>
        /// Gets or sets the new fields
        /// Key: table name
        /// </summary>
        public Dictionary<string, List<DataField>> NewFields { get; set; }

        /// <summary>
        /// Gets or sets the updatable fields
        /// Key: table name
        /// Value => key: old field name
        /// </summary>
        public Dictionary<string, Dictionary<string, DataField>> UpdatableFields { get; set; }

        /// <summary>
        /// Gets or sets the deletable fields
        /// Key: table name
        /// </summary>
        public Dictionary<string, List<string>> DeletableFields { get; set; }
    }
}
