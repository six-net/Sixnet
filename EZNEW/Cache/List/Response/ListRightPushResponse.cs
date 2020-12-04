namespace EZNEW.Cache.List
{
    /// <summary>
    /// List right push response
    /// </summary>
    public class ListRightPushResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length of the list after the push operation.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
