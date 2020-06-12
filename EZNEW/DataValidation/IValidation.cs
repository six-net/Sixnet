using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Validation contract
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the verify result</returns>
        VerifyResult Validate(dynamic data);

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        ValidationAttribute CreateValidationAttribute();
    }
}
