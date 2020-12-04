namespace EZNEW.Cache.String
{
    /// <summary>
    /// String bit count response
    /// </summary>
    public class StringBitCountResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the number of bits set to 1
        /// </summary>
        public long BitNum { get; set; }
    }
}
