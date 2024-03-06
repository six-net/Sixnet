namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List right push result
    /// </summary>
    public class ListRightPushResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the length of the list after the push operation.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
