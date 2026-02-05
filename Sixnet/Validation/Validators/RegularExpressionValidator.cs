// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Regular expression validator
    /// </summary>
    public class RegularExpressionValidator : SixnetBaseValidator
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
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            SetupRegex();
            var value = parameter?.Value;
            var stringValue = value as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return SixnetValidationResult.SuccessResult(parameter?.ErrorMessage);
            }
            var matchResult = Regex.Match(stringValue);
            return matchResult.Success && matchResult.Index == 0 && matchResult.Length == stringValue.Length
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
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
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new RegularExpressionAttribute(Pattern) { ErrorMessage = FormatMessage(parameter.ErrorMessage) });
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = Pattern;

            return rule;
        }
    }
}
