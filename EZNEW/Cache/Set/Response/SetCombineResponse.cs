using System.Collections.Generic;

namespace EZNEW.Cache.Set
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
