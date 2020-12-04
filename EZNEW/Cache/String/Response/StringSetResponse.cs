using System.Collections.Generic;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String value set response
    /// </summary>
    public class StringSetResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the set results
        /// </summary>
        public List<StringEntrySetResult> Results { get; set; }
    }
}
