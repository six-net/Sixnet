namespace EZNEW.Cache.List
{
    /// <summary>
    /// List insert before response
    /// </summary>
    public class ListInsertBeforeResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length of the list after the insert operation, or -1 when the value pivot was not found.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
