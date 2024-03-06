namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List insert before result
    /// </summary>
    public class ListInsertBeforeResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the length of the list after the insert operation, or -1 when the value pivot was not found.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
