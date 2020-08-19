using System;
using System.Collections.Generic;

namespace EZNEW.Upload
{
    /// <summary>
    /// Upload result
    /// </summary>
    [Serializable]
    public class UploadResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the upload was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets response code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets file results
        /// </summary>
        public List<UploadFileResult> Files { get; set; }

        /// <summary>
        /// Gets a default empty upload result
        /// </summary>
        public static readonly UploadResult Empty = new UploadResult();

        #endregion

        #region Methods

        /// <summary>
        /// Combine upload result
        /// </summary>
        /// <param name="results">Other upload results</param>
        /// <returns></returns>
        public UploadResult Combine(params UploadResult[] results)
        {
            if (results == null || results.Length <= 0)
            {
                return this;
            }
            foreach (var result in results)
            {
                if (!result.Success)
                {
                    Success = result.Success;
                    ErrorMessage = result.ErrorMessage;
                    Code = result.Code;
                }
                if (result.Files != null)
                {
                    Files = Files ?? new List<UploadFileResult>();
                    Files.AddRange(result.Files);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets a fail result
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>upload result</returns>
        public static UploadResult FailResult(string message = "")
        {
            return new UploadResult()
            {
                Success = false,
                ErrorMessage = message
            };
        }

        #endregion
    }
}
