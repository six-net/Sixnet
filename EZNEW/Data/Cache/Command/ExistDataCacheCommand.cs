using System;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Check data cache command
    /// </summary>
    public class ExistDataCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the query
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets check from database function
        /// </summary>
        public Func<IQuery, Task<bool>> CheckDatabaseDataProxyAsync { get; set; }

        /// <summary>
        /// Gets or sets the get database data function
        /// </summary>
        public Func<IQuery, Task<T>> GetDatabaseDataProxyAsync { get; set; }
    }
}
