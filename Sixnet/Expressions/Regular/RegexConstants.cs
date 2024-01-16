using System.Text.RegularExpressions;

namespace Sixnet.Expressions.Regular
{
    /// <summary>
    /// Regex constants
    /// </summary>
    public static class RegexConstants
    {
        /// <summary>
        /// Integer regex
        /// </summary>
        public static readonly Regex Integer = new(RegexPatterns.Integer);

        /// <summary>
        /// Positive integer  regex
        /// </summary>
        public static readonly Regex PositiveInteger = new(RegexPatterns.PositiveInteger);

        /// <summary>
        /// Negative integer regex
        /// </summary>
        public static readonly Regex NegativeInteger = new(RegexPatterns.NegativeInteger);

        /// <summary>
        /// Number regex
        /// </summary>
        public static readonly Regex Number = new(RegexPatterns.Number);

        /// <summary>
        /// Positive integer or zero regex
        /// </summary>
        public static readonly Regex PositiveIntegerOrZero = new(RegexPatterns.PositiveIntegerOrZero);

        /// <summary>
        /// Negative integer or zero regex
        /// </summary>
        public static readonly Regex NegativeIntegerOrZero = new(RegexPatterns.NegativeIntegerOrZero);

        /// <summary>
        /// Fraction regex
        /// </summary>
        public static readonly Regex Fraction = new(RegexPatterns.Fraction);

        /// <summary>
        /// Positive fraction regex
        /// </summary>
        public static readonly Regex PositiveFraction = new(RegexPatterns.PositiveFraction);

        /// <summary>
        /// Negative fraction regex
        /// </summary>
        public static readonly Regex NegativeFraction = new(RegexPatterns.NegativeFraction);

        /// <summary>
        /// Positive fraction or zero regex
        /// </summary>
        public static readonly Regex PositiveFractionOrZero = new(RegexPatterns.PositiveFractionOrZero);

        /// <summary>
        /// Negative fraction or zero regex
        /// </summary>
        public static readonly Regex NegativeFractionOrZero = new(RegexPatterns.NegativeFractionOrZero);

        /// <summary>
        /// Email regex
        /// </summary>
        public static readonly Regex Email = new(RegexPatterns.Email);

        /// <summary>
        /// Color regex
        /// </summary>
        public static readonly Regex Color = new(RegexPatterns.Color);

        /// <summary>
        /// Url regex
        /// </summary>
        public static readonly Regex Url = new(RegexPatterns.Url);

        /// <summary>
        /// Contains chinese regex
        /// </summary>
        public static readonly Regex ContainsChinese = new(RegexPatterns.ContainsChinese);

        /// <summary>
        /// All chinese regex
        /// </summary>
        public static readonly Regex AllChinese = new(RegexPatterns.AllChinese);

        /// <summary>
        /// Post code regex
        /// </summary>
        public static readonly Regex PostCode = new(RegexPatterns.PostCode);

        /// <summary>
        /// Mobile regex
        /// </summary>
        public static readonly Regex Mobile = new(RegexPatterns.Mobile);

        /// <summary>
        /// IP v4 regex
        /// </summary>
        public static readonly Regex IPV4 = new(RegexPatterns.IPV4);

        /// <summary>
        /// Image file regex
        /// </summary>
        public static readonly Regex ImageFile = new(RegexPatterns.ImageFie);

        /// <summary>
        /// Compress file regex
        /// </summary>
        public static readonly Regex CompressFile = new(RegexPatterns.CompressFile);

        /// <summary>
        /// Date regex
        /// </summary>
        public static readonly Regex Date = new(RegexPatterns.Date);

        /// <summary>
        /// Datetime regex
        /// </summary>
        public static readonly Regex DateTime = new(RegexPatterns.DateTime);

        /// <summary>
        /// QQ regex
        /// </summary>
        public static readonly Regex QQ = new(RegexPatterns.QQ);

        /// <summary>
        /// Phone regex
        /// </summary>
        public static readonly Regex Phone = new(RegexPatterns.Phone);

        /// <summary>
        /// Letter regex
        /// </summary>
        public static readonly Regex Letter = new(RegexPatterns.Letter);

        /// <summary>
        /// Upper letter regex
        /// </summary>
        public static readonly Regex UpperLetter = new(RegexPatterns.UpperLetter);

        /// <summary>
        /// Lower letter regex
        /// </summary>
        public static readonly Regex LowerLetter = new(RegexPatterns.LowerLetter);

        /// <summary>
        /// Identity card regex
        /// </summary>
        public static readonly Regex IdentityCard = new(RegexPatterns.IdentityCard);

        /// <summary>
        /// Unionpay card regex
        /// </summary>
        public static readonly Regex UnionpayCard = new(RegexPatterns.UnionpayCard);
    }
}
