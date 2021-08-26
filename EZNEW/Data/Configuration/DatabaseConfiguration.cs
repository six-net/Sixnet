using System.Collections.Generic;

namespace EZNEW.Data.Configuration
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
