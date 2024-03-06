using System.Collections.Generic;

namespace Sixnet.Cache.Set.Results
{
    /// <summary>
    /// Set random members result
    /// </summary>
    public class SetRandomMembersResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
