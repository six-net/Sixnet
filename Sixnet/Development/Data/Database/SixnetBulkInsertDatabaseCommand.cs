// "Company © 2025. All rights reserved."

using System.Data;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database bulk insert command
    /// </summary>
    public class SixnetBulkInsertDatabaseCommand : SixnetDatabaseCommand
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
