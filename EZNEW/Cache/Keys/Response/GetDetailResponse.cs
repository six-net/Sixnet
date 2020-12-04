namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Get detail response
    /// </summary>
    public class GetDetailResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the cache entry
        /// </summary>
        public CacheEntry CacheEntry { get; set; }
    }
}
