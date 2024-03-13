using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Regular expression validator
    /// </summary>
    public class RegularExpressionValidator : BaseValidator
    {
        /// <summary>
        /// Gets the mathc pattern
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets or sets the mathc timeout in milliseconds
        /// </summary>
        public int MatchTimeoutInMilliseconds { get; set; } = -1;

        /// <summary>
        /// Gets or sets the regex
        /// </summary>
        private Regex Regex { get; set; }

        /// <summary>
        /// Initialize a regular expression validator
        /// </summary>
        /// <param name="pattern">mathc pattern</param>
        public RegularExpressionValidator(string pattern)
        {
            Pattern = pattern;
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            SetupRegex();
            var stringValue = value as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.ErrorResult(stringValue);
            }
            var matchResult = Regex.Match(stringValue);
            return matchResult.Success && matchResult.Index == 0 && matchResult.Length == stringValue.Length
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Setup regex
        /// </summary>
        void SetupRegex()
        {
            if (Regex == null)
            {
                if (string.IsNullOrEmpty(Pattern))
                {
                    throw new InvalidOperationException($"{nameof(Pattern)} is null or empty");
                }

                Regex = MatchTimeoutInMilliseconds == -1
                    ? new Regex(Pattern)
                    : new Regex(Pattern, default, TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds));
            }
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new RegularExpressionAttribute(Pattern)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
