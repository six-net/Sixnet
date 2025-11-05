// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database migration command
    /// </summary>
    public class MigrationDatabaseCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the migration info
        /// </summary>
        public MigrationInfo MigrationInfo { get; set; }
    }
}
