namespace EZNEW.Cache.List.Response
{
    /// <summary>
    /// List remove response
    /// </summary>
    public class ListRemoveResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the number of removed elements.
        /// </summary>
        public long RemoveCount
        {
            get; set;
        }
    }
}
