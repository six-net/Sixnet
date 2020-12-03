using EZNEW.Data.Cache.Policy;
using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Data cache behavior
    /// </summary>
    public class DataCacheBehavior
    {
        /// <summary>
        /// Gets or sets the exception handling
        /// </summary>
        public DataCacheExceptionHandling ExceptionHandling { get; set; }

        /// <summary>
        /// Get starting result
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Return starting result</returns>
        public StartingResult GetStartingResult(Exception ex)
        {
            switch (ExceptionHandling)
            {
                case DataCacheExceptionHandling.Break:
                    LogManager.LogWarning<DataCacheBehavior>($"Break data access");
                    return StartingResult.Break(ex.Message);
                case DataCacheExceptionHandling.ThrowException:
                    LogManager.LogError<DataCacheBehavior>(ex, ex?.Message ?? string.Empty);
                    throw ex;
                default:
                    LogManager.LogWarning<DataCacheBehavior>($"Continue data access");
                    return StartingResult.Success(ex?.Message ?? string.Empty);
            }
        }

        /// <summary>
        /// Get query data result
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="ex">Exception</param>
        /// <returns>Return query data result</returns>
        public QueryDataResult<T> GetQueryDataResult<T>(Exception ex)
        {
            switch (ExceptionHandling)
            {
                case DataCacheExceptionHandling.Break:
                    LogManager.LogWarning<DataCacheBehavior>($"Break query database");
                    return QueryDataResult<T>.Break(ex.Message, ex);
                case DataCacheExceptionHandling.ThrowException:
                    LogManager.LogError<DataCacheBehavior>(ex, ex?.Message ?? string.Empty);
                    throw ex;
                default:
                    LogManager.LogWarning<DataCacheBehavior>($"Continue query database");
                    return QueryDataResult<T>.Default(exception: ex);
            }
        }

        /// <summary>
        /// Get starting result
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Return starting result</returns>
        public static StartingResult GetStartingResult(DataCacheOperation cacheOperation, Exception ex)
        {
            var behavior = DataCacheManager.Configuration.GetBehavior(new DataCacheBehaviorContext() { CacheOperation = cacheOperation });
            return behavior?.GetStartingResult(ex);
        }

        /// <summary>
        /// Get query data result
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="ex">Exception</param>
        /// <returns>Return query data result</returns>
        public static QueryDataResult<T> GetQueryResult<T>(Exception ex)
        {
            var behavior = DataCacheManager.Configuration.GetBehavior(new DataCacheBehaviorContext() { CacheOperation = DataCacheOperation.QueryData });
            return behavior?.GetQueryDataResult<T>(ex);
        }
    }
}
