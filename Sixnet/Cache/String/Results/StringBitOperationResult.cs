namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String bit operation response
    /// </summary>
    public class StringBitOperationResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the size of the string stored in the destination key
        /// </summary>
        public long DestinationValueLength { get; set; }
    }
}
