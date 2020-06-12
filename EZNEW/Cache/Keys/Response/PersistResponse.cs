namespace EZNEW.Cache.Keys.Response
{
    /// <summary>
    /// Persist response
    /// </summary>
    public class PersistResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets whether persist successful
        /// </summary>
        public bool PersistSuccess
        {
            get; set;
        }
    }
}
