namespace Sixnet.Cache.String.Response
{
    /// <summary>
    /// String set range response
    /// </summary>
    public class StringSetRangeResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the value length after modified
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
