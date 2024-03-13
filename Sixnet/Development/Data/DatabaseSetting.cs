using System;
using System.Collections.Generic;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database setting
    /// </summary>
    public class DatabaseSetting
    {
        /// <summary>
        /// Gets or sets the entity settings
        /// Key:entity type
        /// </summary>
        public Dictionary<Type, EntitySetting> Entities { get; set; }

        /// <summary>
        /// Gets or sets the batch execution options
        /// </summary>
        public DatabaseBatchSetting BatchOptions { get; set; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }
    }
}
