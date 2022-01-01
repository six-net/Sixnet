using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines data options
    /// </summary>
    public class DataOptions
    {
        /// <summary>
        /// Indicates whether deduplication paging data.
        /// The default is false
        /// </summary>
        public bool DeduplicationPagingData { get; set; } = false;

        /// <summary>
        /// Indicates whether deduplication list data.
        /// The default is true.
        /// </summary>
        public bool DeduplicationListData { get; set; } = true;

        /// <summary>
        /// Gets or sets the default page size.
        /// The default is 20.
        /// </summary>
        public int DefaultPageSize { get; set; } = 20;
    }
}
