namespace Sixnet.Cache.List.Response
{
    /// <summary>
    /// List left push response
    /// </summary>
    public class ListLeftPushResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the the length of the list after the push operations.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
