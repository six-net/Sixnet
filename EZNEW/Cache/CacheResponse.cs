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
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response code
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inner responsees
        /// </summary>
        public List<CacheResponse> InnerResponses
        {
            get;
            protected set;
        }

        #endregion

        /// <summary>
        /// Add inner response
        /// </summary>
        /// <param name="innerResponses">Inner responses</param>
        public void AddInnerResponse(params CacheResponse[] innerResponses)
        {
            InnerResponses ??= new List<CacheResponse>();
            if (innerResponses != null && innerResponses.Length > 0)
            {
                InnerResponses.AddRange(innerResponses);
            }
        }

        /// <summary>
        /// Get empty response
        /// </summary>
        /// <returns>Return empty cache response</returns>
        public static CacheResponse Empty()
        {
            return new CacheResponse();
        }

        /// <summary>
        /// Fail response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="code">code</param>
        /// <param name="message">message</param>
        /// <returns>Return data object</returns>
        public static T FailResponse<T>(string code, string message = "") where T : CacheResponse, new()
        {
            T response = new T();
            response.Code = code;
            response.Success = false;
            if (string.IsNullOrWhiteSpace(message))
            {
                CacheCodes.CodeMessages.TryGetValue(code, out message);
            }
            response.Message = message;
            return response;
        }

        /// <summary>
        /// Success response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return data object</returns>
        public static T SuccessResponse<T>() where T : CacheResponse, new()
        {
            T response = new T();
            response.Success = true;
            return response;
        }
    }
}
