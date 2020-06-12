namespace EZNEW.Cache
{
    /// <summary>
    /// Cache query
    /// </summary>
    public class CacheQuery
    {
        protected int page = 1;//page index
        protected int pageSize = 20;//page size

        /// <summary>
        /// Gets or sets the page index
        /// </summary>
        public int Page
        {
            get
            {
                if (page < 1)
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
        /// Gets or sets the page size
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSize < 1)
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
    }
}
