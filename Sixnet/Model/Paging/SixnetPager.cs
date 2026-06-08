// "Company © 2025. All rights reserved."

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
        public static SixnetPagingInfo<T> Create<T>(int pageIndex, int pageSize, int totalCount, IEnumerable<T> datas)
        {
            return new SixnetPagingInfo<T>(pageIndex, pageSize, totalCount, datas);
        }

        /// <summary>
        /// Get a empty paging object
        /// </summary>
        /// <returns>Return a empty paging object</returns>
        public static SixnetPagingInfo<T> Empty<T>()
        {
            return SixnetPagingInfo<T>.Empty();
        }
    }
}
