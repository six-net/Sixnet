using System.Collections.Generic;

namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Sort result
    /// </summary>
    public class SortResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }
    }
}
