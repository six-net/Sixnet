using System.Collections.Generic;

namespace Sixnet.Cache.String.Response
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
