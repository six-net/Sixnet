namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String decrement response
    /// </summary>
    public class StringDecrementResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public decimal NewValue { get; set; }
    }
}
