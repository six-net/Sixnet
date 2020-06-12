namespace EZNEW.Cache.Hash.Response
{
    /// <summary>
    /// Hash delete response
    /// </summary>
    public class HashDeleteResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets whether delete success
        /// </summary>
        public bool DeleteSuccess
        {
            get; set;
        }
    }
}
