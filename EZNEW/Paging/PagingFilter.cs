﻿using System;
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
        protected int page = 1;

        /// <summary>
        /// page size
        /// </summary>
        protected int pageSize = 20;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets page index
        /// </summary>
        public int Page
        {
            get
            {
                if (page <= 0)
                {
                    page = 1;
                }
                return page;
            }
            set
            {
                page = value;
            }
        }

        /// <summary>
        /// Gets or sets page size
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSize <= 0)
                {
                    pageSize = 20;
                }
                return pageSize;
            }
            set
            {
                pageSize = value;
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
