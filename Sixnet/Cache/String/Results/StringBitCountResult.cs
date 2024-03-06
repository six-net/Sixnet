namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String bit count response
    /// </summary>
    public class StringBitCountResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the number of bits set to 1
        /// </summary>
        public long BitNum { get; set; }
    }
}
