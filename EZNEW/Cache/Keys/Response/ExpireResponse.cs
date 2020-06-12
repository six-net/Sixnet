namespace EZNEW.Cache.Keys.Response
{
    /// <summary>
    /// Expire key response
    /// </summary>
    public class ExpireResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the operation result
        /// </summary>
        public bool OperationResult
        {
            get; set;
        }
    }
}
