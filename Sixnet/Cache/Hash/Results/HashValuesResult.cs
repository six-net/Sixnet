using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash values result
    /// </summary>
    public class HashValuesResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<dynamic> Values { get; set; }
    }
}
