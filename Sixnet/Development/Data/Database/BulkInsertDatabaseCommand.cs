// "Company © 2025. All rights reserved."

using System.Data;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database bulk insert command
    /// </summary>
    public class BulkInsertDatabaseCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data table
        /// </summary>
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Gets or sets the bulk insertion options
        /// </summary>
        public ISixnetBulkInsertionOptions BulkInsertionOptions { get; set; }
    }
}
