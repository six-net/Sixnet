using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation contract
    /// </summary>
    public interface ISixnetValidation
    {
        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="useScenario">Use scenario</param>
        /// <returns>Return the verify result</returns>
        ValidationResult Validate(dynamic data, string useScenario = "");

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        ValidationAttribute CreateValidationAttribute();
    }
}
