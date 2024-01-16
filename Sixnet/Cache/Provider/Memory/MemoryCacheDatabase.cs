using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Cache.Provider.Memory
{
    internal class MemoryCacheDatabase : CacheDatabase
    {
        /// <summary>
        /// Gets or sets the data store
        /// </summary>
        internal MemoryCache Store { get; set; }
    }
}
