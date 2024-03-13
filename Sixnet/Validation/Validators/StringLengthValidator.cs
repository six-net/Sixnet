using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// String length validator
    /// </summary>
    public class StringLengthValidator : BaseValidator
    {
        /// <summary>
        /// Initialize a string length validator
        /// </summary>
        /// <param name="maxLength">Max length</param>
        /// <param name="minLength">Min length</param>
        public StringLengthValidator(int maxLength, int minLength = 0)
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
        /// <param name="value">Validate value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage = "")
        {
            // Check the lengths for legality
            EnsureLegalLengths();

            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            // We expect a cast exception if a non-string was passed in.
            var stringValue = value as string;
            if (stringValue == null)
            {
                return ValidationResult.ErrorResult(errorMessage);
            }

            var length = stringValue.Length;
            return length >= MinimumLength && length <= MaximumLength
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
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
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new StringLengthAttribute(MaximumLength)
            {
                MinimumLength = MinimumLength,
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        #endregion
    }
}
