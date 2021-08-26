using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Data;

namespace EZNEW.Paging
{
    /// <summary>
    /// Paging default implement
    /// </summary>
    public class PagingInfo<T>
    {
        #region Constructor

        /// <summary>
        /// Instance a paging object
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total data</param>
        /// <param name="datas">Datas</param>
        public PagingInfo(long pageIndex, int pageSize, long totalCount, IEnumerable<T> datas)
        {
            if (!datas.IsNullOrEmpty())
            {
                Page = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                Items = datas;
                if (PageSize < 1)
                {
                    PageSize = DataManager.DataOptions.DefaultPageSize;
                }
                PageCount = totalCount / PageSize;
                if (totalCount % PageSize > 0)
                {
                    PageCount++;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current page
        /// </summary>
        public long Page { get; set; } = 1;

        /// <summary>
        /// Gets the page size
        /// </summary>
        public int PageSize { get; set; } = DataManager.DataOptions.DefaultPageSize;

        /// <summary>
        /// Gets the page count
        /// </summary>
        public long PageCount { get; set; } = 0;

        /// <summary>
        /// Gets the data total count
        /// </summary>
        public long TotalCount { get; set; } = 0;

        /// <summary>
        /// Gets the datas
        /// </summary>
        public IEnumerable<T> Items
        {
            get; set;
        } = Array.Empty<T>();

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the EZNEW.Paging.Paging<>
        /// </summary>
        /// <returns>A EZNEW.Paging.Paging<>.Enumerator for the EZNEW.Paging.Paging<></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Return a empty paging object
        /// </summary>
        /// <returns></returns>
        public static PagingInfo<T> Empty()
        {
            return new PagingInfo<T>(1, 0, 0, Array.Empty<T>());
        }

        /// <summary>
        /// Converts paging data to a paging object of the specified data type
        /// </summary>
        /// <typeparam name="TTarget">TTarget object type</typeparam>
        /// <returns>Return the target paging object</returns>
        public PagingInfo<TTarget> ConvertTo<TTarget>()
        {
            return new PagingInfo<TTarget>(Page, PageSize, TotalCount, Items.Select(c => c.MapTo<TTarget>()));
        }

        #endregion
    }
}
