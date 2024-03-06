namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String set range response
    /// </summary>
    public class StringSetRangeResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the value length after modified
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
