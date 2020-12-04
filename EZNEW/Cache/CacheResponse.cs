using System.Collections.Generic;
using EZNEW.Cache.Constant;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache response
    /// </summary>
    public class CacheResponse
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
        public CacheServer CacheServer { get; set; }

        /// <summary>
        /// Gets or sets the cache database
        /// </summary>
        public CacheDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get empty response
        /// </summary>
        /// <returns>Return empty cache response</returns>
        public static CacheResponse Empty()
        {
            return new CacheResponse()
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
        public static T FailResponse<T>(string code, string message = "", CacheServer server = null, CacheDatabase database = null) where T : CacheResponse, new()
        {
            T response = new T
            {
                Code = code,
                Success = false,
                CacheServer = server,
                Database = database
            };
            if (string.IsNullOrWhiteSpace(message))
            {
                CacheCodes.CodeMessages.TryGetValue(code, out message);
            }
            response.Message = message;
            return response;
        }

        public static T NoDatabase<T>(CacheServer server) where T : CacheResponse, new()
        {
            return FailResponse<T>("", "No cache database specified", server);
        }

        /// <summary>
        /// Success response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return data object</returns>
        public static T SuccessResponse<T>(CacheServer server = null, CacheDatabase database = null) where T : CacheResponse, new()
        {
            T response = new T
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
