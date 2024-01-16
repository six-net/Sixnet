using System.Collections.Generic;

namespace Sixnet.Cache.Set.Response
{
    /// <summary>
    /// Set combine response
    /// </summary>
    public class SetCombineResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the combine values
        /// </summary>
        public List<string> CombineValues { get; set; }
    }
}
