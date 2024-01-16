using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Response
{
    /// <summary>
    /// Hash keys response
    /// </summary>
    public class HashKeysResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the hash keys
        /// </summary>
        public List<string> HashKeys { get; set; }
    }
}
