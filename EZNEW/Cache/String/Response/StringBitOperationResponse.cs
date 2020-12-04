namespace EZNEW.Cache.String
{
    /// <summary>
    /// String bit operation response
    /// </summary>
    public class StringBitOperationResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the size of the string stored in the destination key
        /// </summary>
        public long DestinationValueLength { get; set; }
    }
}
