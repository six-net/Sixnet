// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// String length validator
    /// </summary>
    public class SixnetStringLengthValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Initialize a string length validator
        /// </summary>
        /// <param name="maxLength">Max length</param>
        /// <param name="minLength">Min length</param>
        public SixnetStringLengthValidator(int maxLength, int minLength = 0)
        {
            MaximumLength = maxLength;
            MinimumLength = minLength;
            defaultErrorMessageValue = string.Format("The character length is between {0} and {1}", minLength, maxLength);
        }

        #region Properties

        /// <summary>
        /// Gets the max length
        /// </summary>
        public int MaximumLength { get; }

        /// <summary>
        /// Gets the min length
        /// </summary>
        public int MinimumLength { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            // Check the lengths for legality
            EnsureLegalLengths();

            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            // We expect a cast exception if a non-string was passed in.
            var value = parameter?.Value;
            var stringValue = value as string;
            parameter.MessageArgs.Add(MinimumLength.ToString());
            parameter.MessageArgs.Add(MaximumLength.ToString());
            if (stringValue == null)
            {
                return SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
            }

            var length = stringValue.Length;
            return length >= MinimumLength && length <= MaximumLength
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Checks that MinimumLength and MaximumLength have legal values.  Throws InvalidOperationException if not.
        /// </summary>
        private void EnsureLegalLengths()
        {
            if (MaximumLength < 0)
            {
                throw new InvalidOperationException("MaximumLength is less 0");
            }

            if (MaximumLength < MinimumLength)
            {
                throw new InvalidOperationException("MaximumLength is less than MinimumLength value");
            }
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new StringLengthAttribute(MaximumLength)
            {
                MinimumLength = MinimumLength,
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            parameter.MessageArgs.Add(MinimumLength.ToString());
            parameter.MessageArgs.Add(MaximumLength.ToString());
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Min = MinimumLength;
            rule.Max = MaximumLength;

            return rule;
        }

        #endregion
    }
}
