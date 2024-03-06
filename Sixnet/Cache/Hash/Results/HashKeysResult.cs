using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash keys result
    /// </summary>
    public class HashKeysResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the hash keys
        /// </summary>
        public List<string> HashKeys { get; set; }
    }
}
