using System;
using EZNEW.Data;
using EZNEW.Development.Query;

namespace EZNEW.Paging
{
    /// <summary>
    /// Paging query condition
    /// </summary>
    [Serializable]
    public class PagingFilter : QueryFilter
    {
        #region Fields

        /// <summary>
        /// page index
        /// </summary>
        protected int pageValue = 1;

        /// <summary>
        /// page size
        /// </summary>
        protected int pageSizeValue = 20;

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
                    pageSizeValue = DataManager.DataOptions.DefaultPageSize;
                }
                return pageSizeValue;
            }
            set
            {
                pageSizeValue = value;
            }
        }

        #endregion

        /// <summary>
        /// Generate a IQuery instance 
        /// </summary>
        /// <returns>Return a IQuery instance</returns>
        public override IQuery CreateQuery()
        {
            return null;
        }
    }
}
