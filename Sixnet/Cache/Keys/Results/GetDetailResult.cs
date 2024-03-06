namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Get detail result
    /// </summary>
    public class GetDetailResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the cache entry
        /// </summary>
        public CacheEntry CacheEntry { get; set; }
    }
}
