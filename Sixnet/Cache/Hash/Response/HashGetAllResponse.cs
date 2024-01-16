using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Response
{
    /// <summary>
    /// Hash get all response
    /// </summary>
    public class HashGetAllResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues { get; set; }
    }
}
