// "Company © 2025. All rights reserved."

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache result
    /// </summary>
    public class SixnetCacheResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether is successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the response code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the cache server
        /// </summary>
        public SixnetCacheServer CacheServer { get; set; }

        /// <summary>
        /// Gets or sets the cache database
        /// </summary>
        public SixnetCacheDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get empty response
        /// </summary>
        /// <returns>Return empty cache response</returns>
        public static SixnetCacheResult Empty()
        {
            return new SixnetCacheResult()
            {
                Code = "0",
                Success = true,
                Message = "Empty response"
            };
        }

        /// <summary>
        /// Fail response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="code">code</param>
        /// <param name="message">message</param>
        /// <returns>Return data object</returns>
        public static T FailResponse<T>(string code, string message = "", SixnetCacheServer server = null, SixnetCacheDatabase database = null) where T : SixnetCacheResult, new()
        {
            var response = new T
            {
                Code = code,
                Success = false,
                CacheServer = server,
                Database = database
            };
            if (string.IsNullOrWhiteSpace(message))
            {
                SixnetCacheCodes.CodeMessages.TryGetValue(code, out message);
            }
            response.Message = message;
            return response;
        }

        public static T NoDatabase<T>(SixnetCacheServer server) where T : SixnetCacheResult, new()
        {
            return FailResponse<T>("", "No cache database specified", server);
        }

        /// <summary>
        /// Success response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return data object</returns>
        public static T SuccessResponse<T>(SixnetCacheServer server = null, SixnetCacheDatabase database = null) where T : SixnetCacheResult, new()
        {
            var response = new T
            {
                Success = true,
                CacheServer = server,
                Database = database
            };
            return response;
        }

        #endregion
    }
}
