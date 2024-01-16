using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload result
    /// </summary>
    [Serializable]
    public class UploadResult
    {
        #region Properties

        /// <summary>
        /// Whether the upload was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets response code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets file results
        /// </summary>
        public List<UploadFileResult> Files { get; set; }

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
                    Message = result.Message;
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
                Message = message
            };
        }

        /// <summary>
        /// Gets a success result
        /// </summary>
        /// <returns></returns>
        public static UploadResult SuccessResult(IEnumerable<UploadFileResult> files = null)
        {
            return new UploadResult()
            {
                Success = true,
                Files = files?.ToList()
            };
        }

        #endregion
    }
}
