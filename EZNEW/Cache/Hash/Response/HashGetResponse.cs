namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash get response
    /// </summary>
    public class HashGetResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the hash value
        /// </summary>
        public dynamic Value { get; set; }
    }
}
