namespace EZNEW.Cache.String.Response
{
    /// <summary>
    /// String set bit response
    /// </summary>
    public class StringSetBitResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the original bit value stored at offset
        /// </summary>
        public bool OldBitValue { get; set; }
    }
}
