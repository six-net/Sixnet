using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Regular expression validator
    /// </summary>
    public class RegularExpressionValidator : DataValidator
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
        public override void Validate(dynamic value, string errorMessage)
        {
            SetupRegex();
            string stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(stringValue))
            {
                SetVerifyResult(true, errorMessage);
                return;
            }
            var matchResult = Regex.Match(stringValue);
            SetVerifyResult((matchResult.Success && matchResult.Index == 0 && matchResult.Length == stringValue.Length), errorMessage);
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
