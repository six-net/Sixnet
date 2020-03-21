using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// verify result
    /// </summary>
    public class VerifyResult
    {
        #region propertys

        /// <summary>
        /// success
        /// </summary>
        public bool Success
        {
            get; set;
        }

        /// <summary>
        /// error message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// field name
        /// </summary>
        public string FieldName
        {
            get; set;
        }

        #endregion

        #region method

        /// <summary>
        /// get error result
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
        /// get success result
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
