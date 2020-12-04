namespace EZNEW.Cache.String
{
    /// <summary>
    /// String bit position response
    /// </summary>
    public class StringBitPositionResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the position
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Gets or sets whether has found value
        /// </summary>
        public bool HasValue { get; set; }
    }
}
