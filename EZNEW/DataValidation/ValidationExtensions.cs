using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EZNEW.DataValidation;
using EZNEW.RegularExpression;

namespace System
{
    /// <summary>
    /// Value validation extensions
    /// </summary>
    public static class ValidationExtensions
    {
        #region Integer

        /// <summary>
        /// Verify whether value is interger
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsInteger(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Integer.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is interger.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsIntegerNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsInteger(value);
        }

        /// <summary>
        /// Verify whether is positive integer
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveInteger(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.PositiveInteger.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is positive integer.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveIntegerNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPositiveInteger(value);
        }

        /// <summary>
        /// Verify whether value is negative integer
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeInteger(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.NegativeInteger.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is negative integer.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeIntegerNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsNegativeInteger(value);
        }

        /// <summary>
        /// Verify whether is positive integer or 0
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveIntegerOrZero(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.PositiveIntegerOrZero.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is positive integer or 0.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveIntegerOrZeroNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPositiveIntegerOrZero(value);
        }

        /// <summary>
        /// Verify whether value is negative integer or 0
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeIntegerOrZero(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.NegativeIntegerOrZero.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is negative integer or 0.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeIntegerOrZeroNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsNegativeIntegerOrZero(value);
        }

        #endregion

        #region Fraction

        /// <summary>
        /// Verify whether is a fraction
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsFraction(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Fraction.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is a fraction.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsFractionNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsFraction(value);
        }

        /// <summary>
        /// Verify whether value is a positive fraction
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveFraction(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.PositiveFraction.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is a positive fraction.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveFractionNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPositiveFraction(value);
        }

        /// <summary>
        /// Verify whether value is a negative fraction
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeFraction(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.NegativeFraction.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is a negative fraction.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeFractionNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsNegativeFraction(value);
        }

        /// <summary>
        /// Verify whether is a positive fraction or 0
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveFractionOrZero(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.PositiveFractionOrZero.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is a positive fraction or 0.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPositiveFractionOrZeroNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPositiveFractionOrZero(value);
        }

        /// <summary>
        /// Verify whether value is a negative fraction or zero
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeFractionOrZero(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.NegativeFractionOrZero.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is a negative fraction or zero.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNegativeFractionOrZeroNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsNegativeFractionOrZero(value);
        }

        #endregion

        #region Number

        /// <summary>
        /// Verify whether value is number
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNumber(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Number.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is number.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsNumberNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsNumber(value);
        }

        #endregion

        #region Email

        /// <summary>
        /// Verify whether value is email address
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsEmail(this string value)
        {
            return !string.IsNullOrEmpty(value) && ValidationConstants.DefaultAttributes.Email.IsValid(value);
        }

        /// <summary>
        /// Verify whether value is email address.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsEmailNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsEmail(value);
        }

        #endregion

        #region Color

        /// <summary>
        ///  Verify whether is color
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsColor(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Color.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is color format value.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsColorNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsColor(value);
        }

        #endregion

        #region Chinese

        /// <summary>
        /// Verify whether value has chinese text
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsContainsChinese(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.ContainsChinese.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value has chinese text.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsContainsChineseNullable(this string value)
        {
            return string.IsNullOrWhiteSpace(value) || IsContainsChinese(value);
        }

        /// <summary>
        /// Verify whether value is chinese lettter
        /// </summary>
        /// <param name="value">char value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsChineseLetter(this char value)
        {
            return RegexConstants.ContainsChinese.IsMatch(value.ToString());
        }

        /// <summary>
        /// Verify whether value is all chinese lettter
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsAllChinese(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.AllChinese.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is all chinese lettter
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsAllChineseNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsAllChinese(value);
        }

        #endregion

        #region Post code

        /// <summary>
        /// Verify whether value is post code
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPostCode(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.PostCode.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is postcode
        /// Allow value is null or empty
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPostCodeNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPostCode(value);
        }

        #endregion

        #region Mobile

        /// <summary>
        /// Verify whether value is a mobile number
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsMobile(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Mobile.IsMatch(value) && value.Length == 11;
        }

        /// <summary>
        /// Verify whether value is a mobile number.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsMobileNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsMobile(value);
        }

        #endregion

        #region IPV4

        /// <summary>
        /// Verify whether value is a ip v4 address
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsIPV4(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.IPV4.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is a ip v4 address.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsIPV4Nullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsIPV4(value);
        }

        #endregion

        #region Date

        /// <summary>
        /// Verify whether value is date format
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsDate(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Date.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is date.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsDateNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsDate(value);
        }

        #endregion

        #region Datetime

        /// <summary>
        /// Verify whether value is datetime
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsDateTime(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.DateTime.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is datetime.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsDateTimeNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsDateTime(value);
        }

        #endregion

        #region QQ

        /// <summary>
        /// Verify whether value is qq format
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsQQ(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.QQ.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is qq format.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsQQNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsQQ(value);
        }

        #endregion

        #region Letter

        /// <summary>
        /// Verify whether value is letter
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsLetter(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.Letter.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is letter.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsLetterNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsLetter(value);
        }

        #endregion

        #region Upper letter

        /// <summary>
        /// Verify whether is upper letter
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsUpperLetter(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.UpperLetter.IsMatch(value);
        }

        /// <summary>
        /// Verify whether is upper letter.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsUpperLetterNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsUpperLetter(value);
        }

        #endregion

        #region Lower letter

        /// <summary>
        /// Verify whether value is lower letter
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsLowerLetter(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.LowerLetter.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is lower letter.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsLowerLetterNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsLowerLetter(value);
        }

        #endregion

        #region Identity card

        /// <summary>
        /// Verify whether value is identity card
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsIdentityCard(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegexConstants.IdentityCard.IsMatch(value);
        }

        /// <summary>
        /// Verify whether value is identity card.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsIdentityCardNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsIdentityCard(value);
        }

        #endregion

        #region Credit card

        /// <summary>
        /// Verify whether is credit card value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsCreditCard(this string value)
        {
            return !string.IsNullOrEmpty(value) && ValidationConstants.DefaultAttributes.CreditCard.IsValid(value);
        }

        /// <summary>
        /// Verify whether is credit card.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsCreditCardNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsCreditCard(value);
        }

        #endregion

        #region Enum

        /// <summary>
        /// Verify whether is enum data
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="enumType">enum type</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsEnumData(this object value, Type enumType)
        {
            var enumDataAttribute = new EnumDataTypeAttribute(enumType);
            return enumDataAttribute.IsValid(value);
        }

        /// <summary>
        /// Verify whether is enum data
        /// </summary>
        /// <typeparam name="TEnum">enum type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsEnumData<TEnum>(this object value)
        {
            return IsEnumData(value, typeof(TEnum));
        }

        #endregion

        #region Maxlength

        /// <summary>
        /// Verify whether value is lessthan or equal special maxlength.
        /// maxLength must be greater than zero.
        /// Allow value is null
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="maxLength">max length,must be greater than zero</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool MaxLength(this object value, int maxLength)
        {
            if (maxLength < 1)
            {
                throw new ArgumentException($"{nameof(maxLength)} must be greater than zero");
            }
            var maxLengthAttribute = new MaxLengthAttribute(maxLength);
            return maxLengthAttribute.IsValid(value);
        }

        #endregion

        #region Minlength

        /// <summary>
        /// Verify whether value is greater than or equal special maxlength.
        /// minLength must be greater than or equal to zero.
        /// Allow value is null
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="minLength">min length,must be greater than or equal to zero</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool MinLength(this object value, int minLength)
        {
            if (minLength < 0)
            {
                throw new ArgumentException($"{nameof(minLength)} must be greater than or equal to zero");
            }
            var minLengthAttribute = new MinLengthAttribute(minLength);
            return minLengthAttribute.IsValid(value);
        }

        #endregion

        #region Phone

        /// <summary>
        /// Verify whether is phone value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPhone(this string value)
        {
            return !string.IsNullOrEmpty(value) && ValidationConstants.DefaultAttributes.Phone.IsValid(value);
        }

        /// <summary>
        /// Verify whether is phone value.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsPhoneNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsPhone(value);
        }

        #endregion

        #region Range

        /// <summary>
        /// Verify whether value is in the range
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="minimum">minimum,(must be less than or equal maximum)</param>
        /// <param name="maximum">maximum,(must be greater than or equal minimum)</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsInRange(this object value, object minimum, object maximum)
        {
            if (value == null)
            {
                return false;
            }
            var rangeAttribute = new RangeAttribute(value.GetType(), minimum?.ToString(), maximum?.ToString());
            return rangeAttribute.IsValid(value);
        }

        /// <summary>
        /// Verify whether value is in the range.
        /// Allow value is null.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="minimum">minimum,(must be less than or equal maximum)</param>
        /// <param name="maximum">maximum,(must be greater than or equal minimum)</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsInRangeNullable(this object value, object minimum, object maximum)
        {
            return value == null || IsInRange(value, minimum, maximum);
        }

        #endregion

        #region File

        #region Image file

        /// <summary>
        /// Verify whether is image file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsImageFile(this string path)
        {
            return !string.IsNullOrEmpty(path) && ValidationConstants.DefaultAttributes.ImageFile.IsValid(path);
        }

        /// <summary>
        /// Verify whether is image file.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsImageFileNullable(this string path)
        {
            return string.IsNullOrEmpty(path) || IsImageFile(path);
        }

        #endregion

        #region Compress file

        /// <summary>
        /// Verify whether is compress file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsCompressFile(this string path)
        {
            return !string.IsNullOrEmpty(path) && ValidationConstants.DefaultAttributes.CompressFile.IsValid(path);
        }

        /// <summary>
        /// Verify whether is compress file.
        /// Allow value is null.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsCompressFileNullable(this string path)
        {
            return string.IsNullOrEmpty(path) || IsCompressFile(path);
        }

        #endregion

        #endregion

        #region Url

        /// <summary>
        /// Verify whether is url
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsUrl(this string value)
        {
            return !string.IsNullOrEmpty(value) && ValidationConstants.DefaultAttributes.Url.IsValid(value);
        }

        /// <summary>
        /// Verify whether is url.
        /// Allow value is null or empty.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return whether the verification has passed</returns>
        public static bool IsUrlNullable(this string value)
        {
            return string.IsNullOrEmpty(value) || IsUrl(value);
        }

        #endregion

        #region Get verify result messages

        /// <summary>
        /// Gets error message
        /// </summary>
        /// <param name="results">Results</param>
        /// <returns>Return the error messages</returns>
        public static List<string> GetErrorMessages(this IEnumerable<VerifyResult> results)
        {
            if (results == null)
            {
                return new List<string>(0);
            }
            List<string> errorMessages = new List<string>();
            foreach (var result in results)
            {
                if (!result.Success)
                {
                    errorMessages.Add($"{result.FieldName}{ValidationManager.FieldErrorMessageSeparator}{result.ErrorMessage}");
                }
            }
            return errorMessages;
        }

        #endregion
    }
}
