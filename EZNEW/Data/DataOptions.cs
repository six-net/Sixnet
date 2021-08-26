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
        /// Indecates whether deduplication paging data.
        /// The default is false
        /// </summary>
        public bool PagingDataMergeForDeduplication { get; set; } = false;

        /// <summary>
        /// Indecates whether deduplication list data.
        /// The default is true.
        /// </summary>
        public bool ListDataMergeForDeduplication { get; set; } = true;

        /// <summary>
        /// Gets or sets the default page size.
        /// The default is 20.
        /// </summary>
        public int DefaultPageSize { get; set; } = 20;
    }
}
