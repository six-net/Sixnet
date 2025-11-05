// "Company © 2025. All rights reserved."

using System.Threading;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database command
    /// </summary>
    public class DatabaseCommand
    {
        /// <summary>
        /// Gets or set the database connection
        /// </summary>
        public DatabaseConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }
    }
}
