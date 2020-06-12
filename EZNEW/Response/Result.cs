using System;

namespace EZNEW.Response
{
    /// <summary>
    /// Operation result
    /// </summary>
    public class Result
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
    public class Result<T> : Result
    {
        #region Properties

        /// <summary>
        /// Gets or sets data object
        /// </summary>
        public T Object
        {
            get
            {
                return (T)Data;
            }
            set
            {
                Data = value;
            }
        }

        #endregion

        #region Methods

        #region Gets a success result

        /// <summary>
        /// Gets a successful result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <returns>Return a successful result</returns>
        public static new Result<T> SuccessResult(string message = "", string code = "", object data = null)
        {
            Result<T> result = new Result<T>();
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
        public static new Result<T> FailedResult(string message = "", string code = "", object data = null)
        {
            Result<T> result = new Result<T>();
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
        public static new Result<T> FailedResult(Exception exception)
        {
            Result<T> result = new Result<T>();
            result.Success = false;
            result.Message = exception.Message;
            return result;
        }

        #endregion

        #endregion
    }
}
