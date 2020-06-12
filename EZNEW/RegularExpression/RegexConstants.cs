using System.Text.RegularExpressions;

namespace EZNEW.RegularExpression
{
    /// <summary>
    /// Regex constants
    /// </summary>
    public static class RegexConstants
    {
        /// <summary>
        /// Integer regex
        /// </summary>
        public static readonly Regex Integer = new Regex(RegexPatterns.Integer);

        /// <summary>
        /// Positive integer  regex
        /// </summary>
        public static readonly Regex PositiveInteger = new Regex(RegexPatterns.PositiveInteger);

        /// <summary>
        /// Negative integer regex
        /// </summary>
        public static readonly Regex NegativeInteger = new Regex(RegexPatterns.NegativeInteger);

        /// <summary>
        /// Number regex
        /// </summary>
        public static readonly Regex Number = new Regex(RegexPatterns.Number);

        /// <summary>
        /// Positive integer or zero regex
        /// </summary>
        public static readonly Regex PositiveIntegerOrZero = new Regex(RegexPatterns.PositiveIntegerOrZero);

        /// <summary>
        /// Negative integer or zero regex
        /// </summary>
        public static readonly Regex NegativeIntegerOrZero = new Regex(RegexPatterns.NegativeIntegerOrZero);

        /// <summary>
        /// Fraction regex
        /// </summary>
        public static readonly Regex Fraction = new Regex(RegexPatterns.Fraction);

        /// <summary>
        /// Positive fraction regex
        /// </summary>
        public static readonly Regex PositiveFraction = new Regex(RegexPatterns.PositiveFraction);

        /// <summary>
        /// Negative fraction regex
        /// </summary>
        public static readonly Regex NegativeFraction = new Regex(RegexPatterns.NegativeFraction);

        /// <summary>
        /// Positive fraction or zero regex
        /// </summary>
        public static readonly Regex PositiveFractionOrZero = new Regex(RegexPatterns.PositiveFractionOrZero);

        /// <summary>
        /// Negative fraction or zero regex
        /// </summary>
        public static readonly Regex NegativeFractionOrZero = new Regex(RegexPatterns.NegativeFractionOrZero);

        /// <summary>
        /// Email regex
        /// </summary>
        public static readonly Regex Email = new Regex(RegexPatterns.Email);

        /// <summary>
        /// Color regex
        /// </summary>
        public static readonly Regex Color = new Regex(RegexPatterns.Color);

        /// <summary>
        /// Url regex
        /// </summary>
        public static readonly Regex Url = new Regex(RegexPatterns.Url);

        /// <summary>
        /// Contains chinese regex
        /// </summary>
        public static readonly Regex ContainsChinese = new Regex(RegexPatterns.ContainsChinese);

        /// <summary>
        /// All chinese regex
        /// </summary>
        public static readonly Regex AllChinese = new Regex(RegexPatterns.AllChinese);

        /// <summary>
        /// Post code regex
        /// </summary>
        public static readonly Regex PostCode = new Regex(RegexPatterns.PostCode);

        /// <summary>
        /// Mobile regex
        /// </summary>
        public static readonly Regex Mobile = new Regex(RegexPatterns.Mobile);

        /// <summary>
        /// IP v4 regex
        /// </summary>
        public static readonly Regex IPV4 = new Regex(RegexPatterns.IPV4);

        /// <summary>
        /// Image file regex
        /// </summary>
        public static readonly Regex ImageFile = new Regex(RegexPatterns.ImageFie);

        /// <summary>
        /// Compress file regex
        /// </summary>
        public static readonly Regex CompressFile = new Regex(RegexPatterns.CompressFile);

        /// <summary>
        /// Date regex
        /// </summary>
        public static readonly Regex Date = new Regex(RegexPatterns.Date);

        /// <summary>
        /// Datetime regex
        /// </summary>
        public static readonly Regex DateTime = new Regex(RegexPatterns.DateTime);

        /// <summary>
        /// QQ regex
        /// </summary>
        public static readonly Regex QQ = new Regex(RegexPatterns.QQ);

        /// <summary>
        /// Phone regex
        /// </summary>
        public static readonly Regex Phone = new Regex(RegexPatterns.Phone);

        /// <summary>
        /// Letter regex
        /// </summary>
        public static readonly Regex Letter = new Regex(RegexPatterns.Letter);

        /// <summary>
        /// Upper letter regex
        /// </summary>
        public static readonly Regex UpperLetter = new Regex(RegexPatterns.UpperLetter);

        /// <summary>
        /// Lower letter regex
        /// </summary>
        public static readonly Regex LowerLetter = new Regex(RegexPatterns.LowerLetter);

        /// <summary>
        /// Identity card regex
        /// </summary>
        public static readonly Regex IdentityCard = new Regex(RegexPatterns.IdentityCard);

        /// <summary>
        /// Unionpay card regex
        /// </summary>
        public static readonly Regex UnionpayCard = new Regex(RegexPatterns.UnionpayCard);
    }
}
