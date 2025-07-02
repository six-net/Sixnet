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
        /// Gets or sets the batch setting
        /// </summary>
        public DatabaseBatchSetting BatchSetting { get; set; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets the default database word and name pattern
        /// </summary>
        public DatabaseWordAndNamePattern DatabaseWordAndNamePattern { get; set; } = DatabaseWordAndNamePattern.Original;

        /// <summary>
        /// Gets or sets the default database word and name separator
        /// </summary>
        public string DatabaseWordAndNameSeparator { get; set; } = "_";
    }
}
