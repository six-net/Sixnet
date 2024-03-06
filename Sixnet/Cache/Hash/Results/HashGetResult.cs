namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash get result
    /// </summary>
    public class HashGetResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the hash value
        /// </summary>
        public dynamic Value { get; set; }
    }
}
