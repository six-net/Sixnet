using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Get data cache command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GetDataCacheCommand<T>
    {
        /// <summary>
        /// Gets or set the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets get database data function
        /// </summary>
        public Func<IQuery, Task<T>> GetDatabaseDataProxyAsync { get; set; }
    }
}
