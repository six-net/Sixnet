namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Key exists result
    /// </summary>
    public class ExistResult : CacheResult
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
