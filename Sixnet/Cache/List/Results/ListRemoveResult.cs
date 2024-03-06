namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List remove result
    /// </summary>
    public class ListRemoveResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the number of removed elements.
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
