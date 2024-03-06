using System.Collections.Generic;

namespace Sixnet.Cache.Set.Results
{
    /// <summary>
    /// Set combine result
    /// </summary>
    public class SetCombineResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the combine values
        /// </summary>
        public List<string> CombineValues { get; set; }
    }
}
