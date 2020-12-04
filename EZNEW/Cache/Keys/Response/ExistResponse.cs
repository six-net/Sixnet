namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Key exists response
    /// </summary>
    public class ExistResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the key count
        /// </summary>
        public long KeyCount { get; set; }

        /// <summary>
        /// Gets whether has key
        /// </summary>
        public bool HasKey
        {
            get
            {
                return KeyCount > 0;
            }
        }
    }
}
