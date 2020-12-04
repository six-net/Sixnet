namespace EZNEW.Cache.String
{
    /// <summary>
    /// String append response
    /// </summary>
    public class StringAppendResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length of the string after the append operation.
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
