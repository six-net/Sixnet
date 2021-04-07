using System;
using Newtonsoft.Json;

namespace EZNEW.Response
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
        #region Fields

        /// <summary>
        /// Default success message
        /// </summary>
        protected const string DefaultSuccessMessage = "success";

        /// <summary>
        /// Default fail message
        /// </summary>
        protected const string DefaultFailedMessage = "failed";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the operation was successful
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
            Result result = new Result();
            result.Success = true;
            result.Code = code;
            result.Data = data;
            result.Message = string.IsNullOrWhiteSpace(message) ? DefaultSuccessMessage : message;
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
            Result result = new Result();
            result.Success = false;
            result.Code = code;
            result.Data = data;
            result.Message = string.IsNullOrWhiteSpace(message) ? DefaultFailedMessage : message;
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Return a failed result</returns>
        public static Result FailedResult(Exception exception)
        {
            Result result = new Result();
            result.Success = false;
            result.Message = exception.Message;
            return result;
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
        #region Fields

        /// <summary>
        /// Default success message
        /// </summary>
        protected const string DefaultSuccessMessage = "success";

        /// <summary>
        /// Default fail message
        /// </summary>
        protected const string DefaultFailedMessage = "failed";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the operation was successful
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
            return new Result<T>()
            {
                Code = code,
                Message = string.IsNullOrWhiteSpace(message) ? DefaultSuccessMessage : message,
                Success = true,
                Data = data
            };
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
            return new Result<T>()
            {
                Code = code,
                Message = string.IsNullOrWhiteSpace(message) ? DefaultSuccessMessage : message,
                Success = false,
                Data = data
            };
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
}
