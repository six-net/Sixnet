using System.Collections.Generic;

namespace Sixnet.Cache.List.Response
{
    /// <summary>
    /// List range response
    /// </summary>
    public class ListRangeResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }
    }
}
