namespace EZNEW.Cache.Set
{
    /// <summary>
    /// Set combine and store response
    /// </summary>
    public class SetCombineAndStoreResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the the number of elements in the resulting set.
        /// </summary>
        public long Count { get; set; }
    }
}
