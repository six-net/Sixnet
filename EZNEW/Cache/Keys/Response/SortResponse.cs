using System.Collections.Generic;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Sort response
    /// </summary>
    public class SortResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }
    }
}
