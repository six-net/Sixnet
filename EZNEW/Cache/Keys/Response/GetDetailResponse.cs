namespace EZNEW.Cache.Keys.Response
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
