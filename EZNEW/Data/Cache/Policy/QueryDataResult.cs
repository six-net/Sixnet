using EZNEW.Cache;
using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Query starting result
    /// </summary>
    public class QueryDataResult<T>
    {
        /// <summary>
        /// Determines whether query database
        /// </summary>
        public bool QueryDatabase { get; set; }

        /// <summary>
        /// Whether queried cache
        /// </summary>
        public bool QuriedCache { get; set; }

        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public List<T> Datas { get; set; }

        /// <summary>
        /// Gets or sets the primary cache keys
        /// </summary>
        public List<CacheKey> PrimaryCacheKeys { get; set; }

        /// <summary>
        /// Gets or sets the other cache keys
        /// </summary>
        public List<CacheKey> OtherCacheKeys { get; set; }

        public static QueryDataResult<T> Default(string message = "", Exception exception = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                LogManager.LogInformation<QueryDataResult<T>>(message);
            }
            if (exception != null)
            {
                LogManager.LogError<QueryDataResult<T>>(exception, exception.Message);
            }
            return new QueryDataResult<T>()
            {
                QueryDatabase = true,
                Datas = new List<T>(0)
            };
        }

        public static QueryDataResult<T> Break(string message = "", Exception ex = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                LogManager.LogError<QueryDataResult<T>>(ex, message);
            }
            return new QueryDataResult<T>()
            {
                QueryDatabase = false
            };
        }
    }
}
