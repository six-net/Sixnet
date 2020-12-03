namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Cache key query
    /// </summary>
    public class KeyQuery : CacheQuery
    {
        /// <summary>
        /// Gets or sets the mate key
        /// </summary>
        public string MateKey { get; set; }

        /// <summary>
        /// Gets or sets the key pattern type
        /// </summary>
        public KeyMatchPattern Type { get; set; }
    }
}
