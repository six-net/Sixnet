namespace EZNEW.Cache
{
    /// <summary>
    /// Cache request option
    /// </summary>
    public interface ICacheOptions
    {
        /// <summary>
        /// Gets or sets the cache object
        /// </summary>
        CacheObject CacheObject { get; set; }

        /// <summary>
        /// Gets or sets the command flags
        /// </summary>
        CacheCommandFlags CommandFlags { get; set; }
    }
}
