using System.Collections.Generic;

namespace Sixnet.Cache.Server.Response
{
    /// <summary>
    /// Gets all data base result
    /// </summary>
    public class GetAllDataBaseResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the databases
        /// </summary>
        public List<CacheDatabase> Databases { get; set; }
    }
}
