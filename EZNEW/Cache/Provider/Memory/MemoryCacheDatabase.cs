using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Cache.Provider.Memory
{
    internal class MemoryCacheDatabase : CacheDatabase
    {
        /// <summary>
        /// Gets or sets the data store
        /// </summary>
        internal MemoryCache Store { get; set; }
    }
}
