using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Get data list cache command
    /// </summary>
    public class GetDataListCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets get database data list function
        /// </summary>
        public Func<IQuery, Task<List<T>>> GetDatabaseDataListProxyAsync { get; set; }
    }
}
