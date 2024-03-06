namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Get keys result
    /// </summary>
    public class GetKeysResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public CachePaging<CacheKey> Keys { get; set; }
    }
}
