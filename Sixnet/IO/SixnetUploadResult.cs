using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.IO
{
    /// <summary>
    /// Sixnet upload result
    /// </summary>
    [Serializable]
    public class SixnetUploadResult
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
        public List<SixnetUploadFileResult> Files { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Combine upload result
        /// </summary>
        /// <param name="results">Other upload results</param>
        /// <returns></returns>
        public SixnetUploadResult Combine(params SixnetUploadResult[] results)
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
                    Files = Files ?? new List<SixnetUploadFileResult>();
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
        public static SixnetUploadResult FailResult(string message = "")
        {
            return new SixnetUploadResult()
            {
                Success = false,
                Message = message
            };
        }

        /// <summary>
        /// Gets a success result
        /// </summary>
        /// <returns></returns>
        public static SixnetUploadResult SuccessResult(IEnumerable<SixnetUploadFileResult> files = null)
        {
            return new SixnetUploadResult()
            {
                Success = true,
                Files = files?.ToList()
            };
        }

        #endregion
    }
}
