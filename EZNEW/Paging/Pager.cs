using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Paging
{
    /// <summary>
    /// Paging manager
    /// </summary>
    public static class Pager
    {
        /// <summary>
        /// Create a paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        /// <param name="datas">Datas</param>
        /// <returns>Return a paging object</returns>
        public static PagingInfo<T> Create<T>(long pageIndex, int pageSize, long totalCount, IEnumerable<T> datas)
        {
            return new PagingInfo<T>(pageIndex, pageSize, totalCount, datas);
        }

        /// <summary>
        /// Get a empty paging object
        /// </summary>
        /// <returns>Return a empty paging object</returns>
        public static PagingInfo<T> Empty<T>()
        {
            return PagingInfo<T>.Empty();
        }
    }
}
