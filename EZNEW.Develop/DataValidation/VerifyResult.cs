using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Verify Result
    /// </summary>
    public class VerifyResult
    {
        #region Propertys

        /// <summary>
        /// Success
        /// </summary>
        public bool Success
        {
            get; set;
        }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Field Name
        /// </summary>
        public string FieldName
        {
            get; set;
        }

        #endregion

        #region Method

        /// <summary>
        /// Get Error Result
        /// </summary>
        /// <param name="errorMessage">error message</param>
        /// <returns></returns>
        public static VerifyResult ErrorResult(string errorMessage = "")
        {
            return new VerifyResult()
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Get Success Result
        /// </summary>
        /// <param name="successMessage">success message</param>
        /// <returns></returns>
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
