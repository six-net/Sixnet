namespace EZNEW.RegularExpression
{
    /// <summary>
    /// Regex patterns
    /// </summary>
    public class RegexPatterns
    {
        /// <summary>
        /// Integer regex pattern string
        /// </summary>
        public const string Integer = @"^-?[1-9]\d*$|^0$";

        /// <summary>
        /// Positive integer regex pattern string
        /// </summary>
        public const string PositiveInteger = @"^[1-9]\d*$";

        /// <summary>
        /// Negative integer regex pattern string
        /// </summary>
        public const string NegativeInteger = @"^-[1-9]\d*$";

        /// <summary>
        /// Number regex pattern string
        /// </summary>
        public static readonly string Number = @"^([+-]?)\d*\.?\d+$";

        /// <summary>
        /// Positive integer or zero regex pattern string
        /// </summary>
        public static readonly string PositiveIntegerOrZero = @"^\d+$";

        /// <summary>
        /// Negative integer or zero regex pattern string
        /// </summary>
        public static readonly string NegativeIntegerOrZero = @"^((-\d+)|(0+))$";

        /// <summary>
        /// Fraction regex pattern string
        /// </summary>
        public static readonly string Fraction = @"^([+-]?)\d*\.\d+$";

        /// <summary>
        /// Positive fraction regex pattern string
        /// </summary>
        public static readonly string PositiveFraction = @"^+?(([1-9]*\.\d+)|(0*\.([1-9]+\d*)|(0*[1-9]+)))$";

        /// <summary>
        /// Negative fraction regex pattern string
        /// </summary>
        public static readonly string NegativeFraction = @"^-(([1-9]*\.\d+)|(0*\.([1-9]+\d*)|(0*[1-9]+)))$";

        /// <summary>
        /// Positive fraction or zero regex pattern string
        /// </summary>
        public static readonly string PositiveFractionOrZero = @"^+?\d*\.\d+$|^[+-]?0+(\.0*)$";

        /// <summary>
        /// Negative fraction or zero regex pattern string
        /// </summary>
        public static readonly string NegativeFractionOrZero = @"^-\d*\.\d+$|^[+-]?0+(\.0*)$";

        /// <summary>
        /// Email regex pattern string
        /// </summary>
        public static readonly string Email = @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$";

        /// <summary>
        /// Color regex pattern string
        /// </summary>
        public static readonly string Color = @"^#[a-fA-F0-9]{6}$";

        /// <summary>
        /// Url regex pattern string
        /// </summary>
        public static readonly string Url = @"^http[s]?:\/\/([\w-]+\.)+[\w-]+([\w-./?%&=]*)?$";

        /// <summary>
        /// Contains chinese text regex pattern string
        /// </summary>
        public static readonly string ContainsChinese = @"^(.|\n)*[\u4E00-\u9FA5\uF900-\uFA2D]+(.|\n)*$";

        /// <summary>
        /// All chinese text regex pattern string
        /// </summary>
        public static readonly string AllChinese = @"^[\u4E00-\u9FA5\uF900-\uFA2D]+[\u4E00-\u9FA5\uF900-\uFA2D]*$";

        /// <summary>
        /// Post code regex pattern string
        /// </summary>
        public static readonly string PostCode = @"^[1-9][0-9]{5}$";

        /// <summary>
        /// Mobile regex pattern string
        /// </summary>
        public static readonly string Mobile = @"^13[0-9]{9}|15[012356789][0-9]{8}|18[0256789][0-9]{8}|147[0-9]{8}$";

        /// <summary>
        /// IP v4 regex pattern string
        /// </summary>
        public static readonly string IPV4 = @"^(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)$";

        /// <summary>
        /// Image file regex pattern string
        /// </summary>
        public static readonly string ImageFie = @"(.*)\.(jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga)$";

        /// <summary>
        /// Compress file regex pattern string
        /// </summary>
        public static readonly string CompressFile = @"(.*)\.(rar|zip|7zip|tgz)$";

        /// <summary>
        /// Date value regex pattern string
        /// </summary>
        public static readonly string Date = @"^\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2}$";

        /// <summary>
        /// Date time regex pattern string
        /// </summary>
        public static readonly string DateTime = @"^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$";

        /// <summary>
        /// QQ regex pattern string
        /// </summary>
        public static readonly string QQ = @"^[1-9][0-9]{4,11}$";

        /// <summary>
        /// Phone regex pattern string
        /// </summary>
        public static readonly string Phone = @"^(([0\+]\d{2,3}-)?(0\d{2,3})-)?(\d{7,8})(-(\d{3,}))?$";

        /// <summary>
        /// Letter regex pattern string
        /// </summary>
        public static readonly string Letter = @"^[A-Za-z]+$";

        /// <summary>
        /// Upper letter regex pattern string
        /// </summary>
        public static readonly string UpperLetter = @"^[A-Z]+$";

        /// <summary>
        /// Lower letter regex pattern string
        /// </summary>
        public static readonly string LowerLetter = @"^[a-z]+$";

        /// <summary>
        /// Identity card regex pattern string
        /// </summary>
        public static readonly string IdentityCard = @"(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)";

        /// <summary>
        /// Unionpay card regex pattern string
        /// </summary>
        public static readonly string UnionpayCard = @"/^62\d{14,17}$/";
    }
}
