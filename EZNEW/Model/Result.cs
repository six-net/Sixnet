using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace EZNEW.Model
{
    public interface IResult
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
    public class Result : IResult
    {
        #region Properties

        /// <summary>
        /// Indicates whether is successfully
        /// </summary>
        [JsonPropertyName(ResultOptions.SuccessField_JsonPropertyName)]
        [JsonProperty(ResultOptions.SuccessField_JsonPropertyName)]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the operation response status code
        /// </summary>
        [JsonPropertyName(ResultOptions.CodeField_JsonPropertyName)]
        [JsonProperty(ResultOptions.CodeField_JsonPropertyName)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the operation response message
        /// </summary>
        [JsonPropertyName(ResultOptions.MessageField_JsonPropertyName)]
        [JsonProperty(ResultOptions.MessageField_JsonPropertyName)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation
        /// </summary>
        [JsonPropertyName(ResultOptions.DataField_JsonPropertyName)]
        [JsonProperty(ResultOptions.DataField_JsonPropertyName)]
        public object Data { get; set; }

        #endregion

        #region Methods

        #region Gets a successful result

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <returns>Return a successful result</returns>
        public static Result SuccessResult(string message = "", string code = "", object data = null)
        {
            Result result = new Result
            {
                Success = true,
                Code = code,
                Data = data,
                Message = message
            };
            ResultOptions.SetResult(result);
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
        /// <returns>Return a failed result</returns>
        public static Result FailedResult(string message = "", string code = "", object data = null)
        {
            Result result = new Result
            {
                Success = false,
                Code = code,
                Data = data,
                Message = message
            };
            ResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Return a failed result</returns>
        public static Result FailedResult(Exception exception)
        {
            return FailedResult(exception?.Message);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a strongly typed operation result
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    public class Result<T> : IResult
    {
        #region Properties

        /// <summary>
        /// Indicates whether is successfully
        /// </summary>
        [JsonPropertyName(ResultOptions.SuccessField_JsonPropertyName)]
        [JsonProperty(ResultOptions.SuccessField_JsonPropertyName)]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the operation response status code
        /// </summary>
        [JsonPropertyName(ResultOptions.CodeField_JsonPropertyName)]
        [JsonProperty(ResultOptions.CodeField_JsonPropertyName)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the operation response message
        /// </summary>
        [JsonPropertyName(ResultOptions.MessageField_JsonPropertyName)]
        [JsonProperty(ResultOptions.MessageField_JsonPropertyName)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation
        /// </summary>
        [JsonPropertyName(ResultOptions.DataField_JsonPropertyName)]
        [JsonProperty(ResultOptions.DataField_JsonPropertyName)]
        public T Data { get; set; }

        #endregion

        #region Methods

        #region Gets a successful result

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <returns>Return a successful result</returns>
        public static Result<T> SuccessResult(T data = default, string message = "", string code = "")
        {
            var result = new Result<T>()
            {
                Code = code,
                Message = message,
                Success = true,
                Data = data
            };
            ResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return a successful result</returns>
        public static Result<T> SuccessResult(string message)
        {
            return SuccessResult(default, message, string.Empty);
        }

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <returns>Return a successful result</returns>
        public static Result<T> SuccessResult(string message, string code)
        {
            return SuccessResult(message: message, code: code);
        }

        #endregion

        #region Gets a failed result

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <returns>Return a failed result</returns>
        public static Result<T> FailedResult(string message = "", string code = "", T data = default)
        {
            var result = new Result<T>()
            {
                Code = code,
                Message = message,
                Success = false,
                Data = data
            };
            ResultOptions.SetResult(result);
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Return a failed result</returns>
        public static Result<T> FailedResult(Exception exception)
        {
            return FailedResult(exception?.Message);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Define result options
    /// </summary>
    public static class ResultOptions
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

        #region json property name

        /// <summary>
        /// Success field json property name
        /// </summary>
        internal const string SuccessField_JsonPropertyName = "success";

        /// <summary>
        /// Code field json property name
        /// </summary>
        internal const string CodeField_JsonPropertyName = "code";

        /// <summary>
        /// Message field json property name
        /// </summary>
        internal const string MessageField_JsonPropertyName = "msg";

        /// <summary>
        /// Data field json property name
        /// </summary>
        internal const string DataField_JsonPropertyName = "data";

        #endregion

        #region methods

        /// <summary>
        /// Set result
        /// </summary>
        /// <param name="originalResult">Original result</param>
        internal static void SetResult(IResult originalResult)
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
