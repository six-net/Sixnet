using System.Collections.Generic;

namespace EZNEW.Cache.String.Response
{
    /// <summary>
    /// String get response
    /// </summary>
    public class StringGetResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<CacheEntry> Values { get; set; }
    }
}
