using System;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache operation options
    /// </summary>
    public interface ISixnetCacheOperationOptions
    {
        /// <summary>
        /// Gets or sets the cache object
        /// </summary>
        CacheObject CacheObject { get; set; }

        /// <summary>
        /// Gets or sets the command flags
        /// </summary>
        CacheCommandFlags CommandFlags { get; set; }

        /// <summary>
        /// Gets or sets the cache structure pattern
        /// </summary>
        CacheStructurePattern StructurePattern { get; set; }

        ///// <summary>
        ///// Whether in memory cache first
        ///// </summary>
        //bool InMemoryFirst { get; set; }

        /// <summary>
        /// Whether use in memory for default
        /// </summary>
        bool UseInMemoryForDefault { get; set; }
    }
}
