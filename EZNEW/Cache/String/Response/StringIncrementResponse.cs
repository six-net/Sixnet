namespace EZNEW.Cache.String.Response
{
    /// <summary>
    /// String increment response
    /// </summary>
    public class StringIncrementResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public long NewValue { get; set; }
    }
}
