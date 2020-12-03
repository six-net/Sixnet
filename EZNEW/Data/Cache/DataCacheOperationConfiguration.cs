using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Data cache operation configuration
    /// </summary>
    public class DataCacheOperationConfiguration
    {
        /// <summary>
        /// Gets or sets whether execute cache operation synchronous
        /// </summary>
        public bool Synchronous { get; set; }

        /// <summary>
        /// Gets or sets the cache operation trigger time
        /// </summary>
        public DataCacheOperationTriggerTime TriggerTime { get; set; }
    }
}
