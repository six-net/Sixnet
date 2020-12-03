using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Get cache data result
    /// </summary>
    public class GetCacheDataResult<T>
    {
        public static readonly GetCacheDataResult<T> Default = null;

        static GetCacheDataResult()
        {
            Default = new GetCacheDataResult<T>()
            {
                NotQueryDatabase = false
            };
        }

        /// <summary>
        /// Gets or sets whether needn't to query database
        /// </summary>
        public bool NotQueryDatabase { get; set; }

        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public List<T> Datas { get; set; }
    }
}
