using System.Collections.Generic;

namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Gets all data base response
    /// </summary>
    public class GetAllDataBaseResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the databases
        /// </summary>
        public List<CacheDatabase> Databases { get; set; }
    }
}
