using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Data;
using Newtonsoft.Json;

namespace EZNEW.Paging
{
    /// <summary>
    /// Paging default implement
    /// </summary>
    [JsonObject]
    public class PagingInfo<T> : IEnumerable<T>
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
                    PageSize = DataManager.DefaultPageSize;
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
        public long Page { get; } = 1;

        /// <summary>
        /// Gets the page size
        /// </summary>
        public int PageSize { get; } = DataManager.DefaultPageSize;

        /// <summary>
        /// Gets the page count
        /// </summary>
        public long PageCount { get; } = 0;

        /// <summary>
        /// Gets the data total count
        /// </summary>
        public long TotalCount { get; } = 0;

        /// <summary>
        /// Gets the datas
        /// </summary>
        public IEnumerable<T> Items
        {
            get;
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
        /// Returns an enumerator that iterates through the EZNEW.Paging.Paging<>
        /// </summary>
        /// <returns>A EZNEW.Paging.Paging<>.Enumerator for the EZNEW.Paging.Paging<></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            return new PagingInfo<TTarget>(Page, PageSize, TotalCount, this.Select(c => c.MapTo<TTarget>()));
        }

        #endregion
    }
}
