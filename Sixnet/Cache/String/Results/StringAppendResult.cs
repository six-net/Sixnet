namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String append result
    /// </summary>
    public class StringAppendResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the length of the string after the append operation.
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
