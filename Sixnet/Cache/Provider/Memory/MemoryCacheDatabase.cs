// "Company © 2025. All rights reserved."

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
