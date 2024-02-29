using System.Collections.Generic;

namespace Sixnet.Model.Paging
{
    /// <summary>
    /// Pager
    /// </summary>
    public static class SixnetPager
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
        public static PagingInfo<T> Create<T>(int pageIndex, int pageSize, int totalCount, IEnumerable<T> datas)
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
