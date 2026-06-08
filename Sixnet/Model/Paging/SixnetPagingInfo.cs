// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;

namespace Sixnet.Model.Paging
{
    /// <summary>
    /// Paging default implement
    /// </summary>
    public class SixnetPagingInfo<T>
    {
        #region Constructor

        /// <summary>
        /// Instance a paging object
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total data</param>
        /// <param name="datas">Datas</param>
        public SixnetPagingInfo(int pageIndex, int pageSize, int totalCount, IEnumerable<T> datas)
        {
            if (!datas.IsNullOrEmpty())
            {
                Page = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                Items = datas?.ToList() ?? new List<T>(0);
                if (PageSize < 1)
                {
                    PageSize = SixnetDataManager.GetDefaultPagingSize();
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
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets the page size
        /// </summary>
        public int PageSize { get; set; } = SixnetDataManager.GetDefaultPagingSize();

        /// <summary>
        /// Gets the page count
        /// </summary>
        public int PageCount { get; set; } = 0;

        /// <summary>
        /// Gets the data total count
        /// </summary>
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// Gets the datas
        /// </summary>
        public List<T> Items
        {
            get; set;
        } = new List<T>(0);

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the Sixnet.Paging.Paging<>
        /// </summary>
        /// <returns>A Sixnet.Paging.Paging<>.Enumerator for the Sixnet.Paging.Paging<></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Return a empty paging object
        /// </summary>
        /// <returns></returns>
        public static SixnetPagingInfo<T> Empty()
        {
            return new SixnetPagingInfo<T>(1, 0, 0, Array.Empty<T>());
        }

        /// <summary>
        /// Converts paging data to a paging object of the specified data type
        /// </summary>
        /// <typeparam name="TTarget">TTarget object type</typeparam>
        /// <returns>Return the target paging object</returns>
        public SixnetPagingInfo<TTarget> ConvertTo<TTarget>()
        {
            return new SixnetPagingInfo<TTarget>(Page, PageSize, TotalCount, Items.Select(c => c.MapTo<TTarget>()).ToList());
        }

        #endregion
    }
}
