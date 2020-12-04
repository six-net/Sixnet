namespace EZNEW.Cache.List
{
    /// <summary>
    /// List insert after response
    /// </summary>
    public class ListInsertAfterResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length of the list after the insert operation, or -1 when the value pivot was not found.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
