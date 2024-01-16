using System.Collections.Generic;

namespace Sixnet.Development.Data.Configuration
{
    /// <summary>
    /// Database configuration
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Gets or sets the server type configuration
        /// </summary>
        public Dictionary<DatabaseServerType, DatabaseServerConfiguration> Servers { get; set; }
    }
}
