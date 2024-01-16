namespace Sixnet.Cache
{
    /// <summary>
    /// Cache query
    /// </summary>
    public class CacheQuery
    {
        protected int pageValue = 1;//page index
        protected int pageSizeValue = 20;//page size

        /// <summary>
        /// Gets or sets the page index
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
        /// Gets or sets the page size
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSizeValue < 1)
                {
                    pageSizeValue = 20;
                }
                return pageSizeValue;
            }
            set
            {
                pageSizeValue = value;
            }
        }
    }
}
