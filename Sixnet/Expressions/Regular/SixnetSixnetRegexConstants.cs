// "Company © 2025. All rights reserved."

using System.Text.RegularExpressions;

namespace Sixnet.Expressions.Regular
{
    /// <summary>
    /// Regex constants
    /// </summary>
    public static class SixnetSixnetRegexConstants
    {
        /// <summary>
        /// Integer regex
        /// </summary>
        public static readonly Regex Integer = new(SixnetRegexPatterns.Integer);

        /// <summary>
        /// Positive integer  regex
        /// </summary>
        public static readonly Regex PositiveInteger = new(SixnetRegexPatterns.PositiveInteger);

        /// <summary>
        /// Negative integer regex
        /// </summary>
        public static readonly Regex NegativeInteger = new(SixnetRegexPatterns.NegativeInteger);

        /// <summary>
        /// Number regex
        /// </summary>
        public static readonly Regex Number = new(SixnetRegexPatterns.Number);

        /// <summary>
        /// Positive integer or zero regex
        /// </summary>
        public static readonly Regex PositiveIntegerOrZero = new(SixnetRegexPatterns.PositiveIntegerOrZero);

        /// <summary>
        /// Negative integer or zero regex
        /// </summary>
        public static readonly Regex NegativeIntegerOrZero = new(SixnetRegexPatterns.NegativeIntegerOrZero);

        /// <summary>
        /// Fraction regex
        /// </summary>
        public static readonly Regex Fraction = new(SixnetRegexPatterns.Fraction);

        /// <summary>
        /// Positive fraction regex
        /// </summary>
        public static readonly Regex PositiveFraction = new(SixnetRegexPatterns.PositiveFraction);

        /// <summary>
        /// Negative fraction regex
        /// </summary>
        public static readonly Regex NegativeFraction = new(SixnetRegexPatterns.NegativeFraction);

        /// <summary>
        /// Positive fraction or zero regex
        /// </summary>
        public static readonly Regex PositiveFractionOrZero = new(SixnetRegexPatterns.PositiveFractionOrZero);

        /// <summary>
        /// Negative fraction or zero regex
        /// </summary>
        public static readonly Regex NegativeFractionOrZero = new(SixnetRegexPatterns.NegativeFractionOrZero);

        /// <summary>
        /// Email regex
        /// </summary>
        public static readonly Regex Email = new(SixnetRegexPatterns.Email);

        /// <summary>
        /// Color regex
        /// </summary>
        public static readonly Regex Color = new(SixnetRegexPatterns.Color);

        /// <summary>
        /// Url regex
        /// </summary>
        public static readonly Regex Url = new(SixnetRegexPatterns.Url);

        /// <summary>
        /// Contains chinese regex
        /// </summary>
        public static readonly Regex ContainsChinese = new(SixnetRegexPatterns.ContainsChinese);

        /// <summary>
        /// All chinese regex
        /// </summary>
        public static readonly Regex AllChinese = new(SixnetRegexPatterns.AllChinese);

        /// <summary>
        /// Post code regex
        /// </summary>
        public static readonly Regex PostCode = new(SixnetRegexPatterns.PostCode);

        /// <summary>
        /// Mobile regex
        /// </summary>
        public static readonly Regex Mobile = new(SixnetRegexPatterns.Mobile);

        /// <summary>
        /// IP v4 regex
        /// </summary>
        public static readonly Regex IPV4 = new(SixnetRegexPatterns.IPV4);

        /// <summary>
        /// Image file regex
        /// </summary>
        public static readonly Regex ImageFile = new(SixnetRegexPatterns.ImageFie);

        /// <summary>
        /// Compress file regex
        /// </summary>
        public static readonly Regex CompressFile = new(SixnetRegexPatterns.CompressFile);

        /// <summary>
        /// Date regex
        /// </summary>
        public static readonly Regex Date = new(SixnetRegexPatterns.Date);

        /// <summary>
        /// Datetime regex
        /// </summary>
        public static readonly Regex DateTime = new(SixnetRegexPatterns.DateTime);

        /// <summary>
        /// QQ regex
        /// </summary>
        public static readonly Regex QQ = new(SixnetRegexPatterns.QQ);

        /// <summary>
        /// Phone regex
        /// </summary>
        public static readonly Regex Phone = new(SixnetRegexPatterns.Phone);

        /// <summary>
        /// Letter regex
        /// </summary>
        public static readonly Regex Letter = new(SixnetRegexPatterns.Letter);

        /// <summary>
        /// Upper letter regex
        /// </summary>
        public static readonly Regex UpperLetter = new(SixnetRegexPatterns.UpperLetter);

        /// <summary>
        /// Lower letter regex
        /// </summary>
        public static readonly Regex LowerLetter = new(SixnetRegexPatterns.LowerLetter);

        /// <summary>
        /// Identity card regex
        /// </summary>
        public static readonly Regex IdentityCard = new(SixnetRegexPatterns.IdentityCard);

        /// <summary>
        /// Unionpay card regex
        /// </summary>
        public static readonly Regex UnionpayCard = new(SixnetRegexPatterns.UnionpayCard);
    }
}
