using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Development.Query;
using EZNEW.Paging;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Get data paging cache command
    /// </summary>
    public class GetDataPagingCacheCommand<T> : GetDataListCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets get database data paging function
        /// </summary>
        public Func<IQuery, Task<PagingInfo<T>>> GetDatabaseDataPagingProxyAsync { get; set; }
    }
}
