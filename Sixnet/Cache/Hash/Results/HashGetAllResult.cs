using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash get all result
    /// </summary>
    public class HashGetAllResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues { get; set; }
    }
}
