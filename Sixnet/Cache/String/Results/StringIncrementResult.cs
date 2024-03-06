namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String increment response
    /// </summary>
    public class StringIncrementResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public long NewValue { get; set; }
    }
}
