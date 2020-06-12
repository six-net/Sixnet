using System.Collections.Generic;

namespace EZNEW.Cache.Hash.Response
{
    /// <summary>
    /// Hash scan response
    /// </summary>
    public class HashScanResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues
        {
            get; set;
        }
    }
}
