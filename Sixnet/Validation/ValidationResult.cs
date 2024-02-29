using System;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation result
    /// </summary>
    [Serializable]
    public class ValidationResult
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
        public static ValidationResult ErrorResult(string errorMessage = "")
        {
            return new ValidationResult()
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
        public static ValidationResult SuccessResult(string successMessage = "")
        {
            return new ValidationResult()
            {
                Success = true,
                ErrorMessage = successMessage
            };
        }

        #endregion
    }
}
