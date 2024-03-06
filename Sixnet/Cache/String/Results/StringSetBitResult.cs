namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String set bit response
    /// </summary>
    public class StringSetBitResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the original bit value stored at offset
        /// </summary>
        public bool OldBitValue { get; set; }
    }
}
