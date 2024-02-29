using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database command
    /// </summary>
    public class SixnetDatabaseCommand
    {
        /// <summary>
        /// Gets or set the database connection
        /// </summary>
        public SixnetDatabaseConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }
    }
}
