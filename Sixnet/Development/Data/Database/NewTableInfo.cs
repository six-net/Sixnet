// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// New table info
    /// </summary>
    public class NewTableInfo
    {
        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the table names
        /// </summary>
        public List<DatabaseObjectName> TableNames { get; set; }
    }
}
