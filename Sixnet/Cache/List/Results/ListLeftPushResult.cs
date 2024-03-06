namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List left push result
    /// </summary>
    public class ListLeftPushResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the the length of the list after the push operations.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
