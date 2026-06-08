// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database migration command
    /// </summary>
    public class SixnetMigrationDatabaseCommand : SixnetDatabaseCommand
    {
        /// <summary>
        /// Gets or sets the migration info
        /// </summary>
        public SixnetMigrationInfo MigrationInfo { get; set; }
    }
}
