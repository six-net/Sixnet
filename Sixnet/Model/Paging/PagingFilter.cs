using System;
using Sixnet.Development.Data;
using Sixnet.Development.Queryable;

namespace Sixnet.Model.Paging
{
    /// <summary>
    /// Paging query condition
    /// </summary>
    [Serializable]
    public class PagingFilter : QueryableFilter
    {
        #region Fields

        /// <summary>
        /// page index
        /// </summary>
        protected int pageValue = 1;

        /// <summary>
        /// page size
        /// </summary>
        protected int pageSizeValue = SixnetDataManager.DefaultPagingSize;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets page index
        /// </summary>
        public int Page
        {
            get
            {
                if (pageValue < 1)
                {
                    pageValue = 1;
                }
                return pageValue;
            }
            set
            {
                pageValue = value;
            }
        }

        /// <summary>
        /// Gets or sets page size
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSizeValue < 1)
                {
                    pageSizeValue = SixnetDataManager.DefaultPagingSize;
                }
                return pageSizeValue;
            }
            set
            {
                pageSizeValue = value;
            }
        }

        #endregion

        #region Generate queryable

        /// <summary>
        /// Generate queryable
        /// </summary>
        /// <param name="useForPaging">Whether use for paging</param>
        /// <returns></returns>
        public override ISixnetQueryable CreateQueryable(bool useForPaging = false)
        {
            return null;
        } 

        #endregion

        #region Create

        /// <summary>
        ///  Create a paging filter
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public static PagingFilter Create(int page, int pageSize)
        {
            return new PagingFilter()
            {
                Page = page,
                PageSize = pageSize
            };
        }

        #endregion
    }
}
