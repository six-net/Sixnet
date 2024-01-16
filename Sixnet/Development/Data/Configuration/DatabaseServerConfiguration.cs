using System;
using System.Collections.Generic;

namespace Sixnet.Development.Data.Configuration
{
    /// <summary>
    /// Database server configuration
    /// </summary>
    public class DatabaseServerConfiguration
    {
        /// <summary>
        /// Gets or sets the database provider type full name
        /// </summary>
        public string DatabaseProviderFullTypeName { get; set; }

        /// <summary>
        /// Gets or sets the entity configuration
        /// Key:entity type
        /// </summary>
        public Dictionary<Type, DataEntityConfiguration> EntityConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the batch execution configuration
        /// </summary>
        public BatchExecutionConfiguration BatchExecutionConfiguration { get; set; }
    }
}
