namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String get set response
    /// </summary>
    public class StringGetSetResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the old value
        /// </summary>
        public string OldValue { get; set; }
    }
}
