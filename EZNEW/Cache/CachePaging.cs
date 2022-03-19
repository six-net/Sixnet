using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache paging
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class CachePaging<T> : IEnumerable<T>
    {
        private T[] items = new T[0];//datas

        #region Constructor

        /// <summary>
        /// Instance a paging object
        /// </summary>
        /// <param name="page">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="totalCount">total data</param>
        /// <param name="items">datas</param>
        public CachePaging(long page, long pageSize, long totalCount, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            this.items = items.ToArray();
            if (pageSize > 0)
            {
                PageCount = totalCount / pageSize;
                if (totalCount % pageSize > 0)
                {
                    PageCount++;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public long Page { get; set; }

        /// <summary>
        /// Gets or sets the total count
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the page count
        /// </summary>
        public long PageCount { get; set; }

        #endregion

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < items.Length; i++)
            {
                yield return items[i];
            }
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Return a Empty Paging Object
        /// </summary>
        /// <returns></returns>
        public static CachePaging<T> EmptyPaging()
        {
            return new CachePaging<T>(1, 0, 0, null);
        }
    }
}
