namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String bit position response
    /// </summary>
    public class StringBitPositionResult : CacheResult
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
