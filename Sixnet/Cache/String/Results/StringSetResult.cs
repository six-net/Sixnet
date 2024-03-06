using System.Collections.Generic;

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String value set response
    /// </summary>
    public class StringSetResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the set results
        /// </summary>
        public List<StringEntrySetResult> Results { get; set; }
    }
}
