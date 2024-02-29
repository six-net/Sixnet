using Sixnet.App;
using Sixnet.Localization;
using System;

namespace Sixnet.Model
{
    public interface ISixnetResult
    {
        /// <summary>
        /// Gets or sets whether the operation was successful
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the operation response status code
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Gets or sets the operation response message
        /// </summary>
        string Message { get; set; }
    }

    /// <summary>
    /// Operation result
    /// </summary>
    public class SixnetResult : ISixnetResult
    {
        #region Properties

        /// <summary>
        /// Indicates whether is successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the operation response status code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the operation response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation
        /// </summary>
        public object Data { get; set; }

        #endregion

        #region Constructor

        private SixnetResult() { }

        #endregion

        #region Methods

        #region Gets a successful result

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a successful result</returns>
        public static SixnetResult SuccessResult(string message = "", string code = "", object data = null, params string[] messageArgs)
        {
            var result = new SixnetResult
            {
                Success = true,
                Code = code,
                Data = data,
                Message = SixnetLocalizer.IsLocalizeThrowMessage() ? message.Localize(messageArgs) : message
            };
            SixnetResultOptions.SetResult(result);
            return result;
        }

        #endregion

        #region Gets a failed result

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a failed result</returns>
        public static SixnetResult FailedResult(string message = "", string code = "", object data = null, params string[] messageArgs)
        {
            var result = new SixnetResult
            {
                Success = false,
                Code = code,
                Data = data,
                Message = SixnetLocalizer.IsLocalizeThrowMessage() ? message.Localize(messageArgs) : message
            };
            SixnetResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a failed result</returns>
        public static SixnetResult FailedResult(Exception exception, params string[] messageArgs)
        {
            return FailedResult(exception?.Message, messageArgs: messageArgs);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a strongly typed operation result
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    public class SixnetResult<T> : ISixnetResult
    {
        #region Properties

        /// <summary>
        /// Indicates whether is successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the operation response status code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the operation response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation
        /// </summary>
        public T Data { get; set; }

        #endregion

        #region Constructor

        private SixnetResult() { }

        #endregion

        #region Methods

        #region Gets a successful result

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a successful result</returns>
        public static SixnetResult<T> SuccessResult(T data = default, string message = "", string code = "", params string[] messageArgs)
        {
            var result = new SixnetResult<T>()
            {
                Code = code,
                Message = SixnetLocalizer.IsLocalizeThrowMessage() ? message.Localize(messageArgs) : message,
                Success = true,
                Data = data
            };
            SixnetResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a successful result</returns>
        public static SixnetResult<T> SuccessResult(string message, params string[] messageArgs)
        {
            return SuccessResult(default, message, string.Empty, messageArgs);
        }

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a successful result</returns>
        public static SixnetResult<T> SuccessResult(string message, string code, params string[] messageArgs)
        {
            return SuccessResult(default, message, code, messageArgs);
        }

        #endregion

        #region Gets a failed result

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a failed result</returns>
        public static SixnetResult<T> FailedResult(string message = "", string code = "", T data = default, params string[] messageArgs)
        {
            var result = new SixnetResult<T>()
            {
                Code = code,
                Message = SixnetLocalizer.IsLocalizeThrowMessage() ? message.Localize(messageArgs) : message,
                Success = false,
                Data = data
            };
            SixnetResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="messageArgs">Message args</param>
        /// <returns>Return a failed result</returns>
        public static SixnetResult<T> FailedResult(Exception exception, params string[] messageArgs)
        {
            return FailedResult(exception?.Message, "", default, messageArgs);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Define result options
    /// </summary>
    public static class SixnetResultOptions
    {
        #region message

        /// <summary>
        /// Default success message
        /// </summary>
        public static string DefaultSuccessMessage = "success";

        /// <summary>
        /// Default fail message
        /// </summary>
        public static string DefaultFailedMessage = "failed";

        #endregion

        #region code

        /// <summary>
        /// Default success code
        /// </summary>
        public static string DefaultSuccessCode = "200";

        /// <summary>
        /// Default failed code
        /// </summary>
        public static string DefaultFailedCode = "500";

        #endregion

        #region methods

        /// <summary>
        /// Set result
        /// </summary>
        /// <param name="originalResult">Original result</param>
        internal static void SetResult(ISixnetResult originalResult)
        {
            if (originalResult != null)
            {
                if (string.IsNullOrWhiteSpace(originalResult.Code))
                {
                    originalResult.Code = originalResult.Success ? DefaultSuccessCode : DefaultFailedCode;
                }
                if (string.IsNullOrWhiteSpace(originalResult.Message))
                {
                    originalResult.Message = originalResult.Success ? DefaultSuccessMessage : DefaultFailedMessage;
                }
            }
        }

        #endregion
    }
}
