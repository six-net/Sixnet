using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database migration command
    /// </summary>
    public class MigrationDatabaseCommand : SixnetDatabaseCommand
    {
        /// <summary>
        /// Gets or sets the migration info
        /// </summary>
        public MigrationInfo MigrationInfo { get; set; }
    }
}
