using System.Collections.Generic;

namespace Sixnet.Cache.Set.Results
{
    /// <summary>
    /// Set members result
    /// </summary>
    public class SetMembersResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
