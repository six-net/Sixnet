using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database bulk insert command
    /// </summary>
    public class DatabaseBulkInsertCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data table
        /// </summary>
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Gets or sets the bulk insertion options
        /// </summary>
        public IBulkInsertionOptions BulkInsertionOptions { get; set; }
    }
}
