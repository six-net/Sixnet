using System.Collections.Generic;

namespace EZNEW.Cache.Set.Response
{
    /// <summary>
    /// Set random members response
    /// </summary>
    public class SetRandomMembersResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
