using System.Collections.Generic;

namespace Sixnet.Cache.Set.Response
{
    /// <summary>
    /// Set members response
    /// </summary>
    public class SetMembersResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
