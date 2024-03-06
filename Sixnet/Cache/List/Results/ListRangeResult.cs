using System.Collections.Generic;

namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List range result
    /// </summary>
    public class ListRangeResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }
    }
}
