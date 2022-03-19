using System;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Verify result
    /// </summary>
    [Serializable]
    public class VerifyResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether verify successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets error result
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Return verify result</returns>
        public static VerifyResult ErrorResult(string errorMessage = "")
        {
            return new VerifyResult()
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Gets success result
        /// </summary>
        /// <param name="successMessage">Success message</param>
        /// <returns>Return verify result</returns>
        public static VerifyResult SuccessResult(string successMessage = "")
        {
            return new VerifyResult()
            {
                Success = true,
                ErrorMessage = successMessage
            };
        }

        #endregion
    }
}
