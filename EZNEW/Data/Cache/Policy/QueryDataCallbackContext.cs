using EZNEW.Cache;
using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Query data call context
    /// </summary>
    public class QueryDataCallbackContext<T>
    {
        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public IEnumerable<T> Datas { get; set; }

        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the primary cache keys
        /// </summary>
        public List<CacheKey> PrimaryCacheKeys { get; set; }

        /// <summary>
        /// Gets or sets the other cache keys
        /// </summary>
        public List<CacheKey> OtherCacheKeys { get; set; }
    }
}
