// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Default field formatter names
    /// </summary>
    public static class SixnetFieldFormatterNames
    {
        /// <summary>
        /// formatter name prefix
        /// </summary>
        private const string FormatterNamePrefix = "SIXNET_FIELDCONVERSION_";

        /// <summary>
        /// Calculate char length
        /// </summary>
        public const string CHARLENGTH = FormatterNamePrefix + "CHARLENGTH";

        /// <summary>
        /// Count data
        /// </summary>
        public const string COUNT = FormatterNamePrefix + "COUNT";

        /// <summary>
        /// Max value
        /// </summary>
        public const string MAX = FormatterNamePrefix + "MAX";

        /// <summary>
        /// Min value
        /// </summary>
        public const string MIN = FormatterNamePrefix + "MIN";

        /// <summary>
        /// Sum value
        /// </summary>
        public const string SUM = FormatterNamePrefix + "SUM";

        /// <summary>
        /// Avg value
        /// </summary>
        public const string AVG = FormatterNamePrefix + "AVG";

        /// <summary>
        /// Json value
        /// </summary>
        public const string JSON_VALUE = FormatterNamePrefix + "JSON_VALUE";

        /// <summary>
        /// Json object
        /// </summary>
        public const string JSON_OBJECT = FormatterNamePrefix + "JSON_OBJECT";
        /// <summary>
        /// And
        /// </summary>
        public const string AND = FormatterNamePrefix + "AND";
        /// <summary>
        /// Or
        /// </summary>
        public const string OR = FormatterNamePrefix + "OR";
        /// <summary>
        /// Not
        /// </summary>
        public const string NOT = FormatterNamePrefix + "NOT";
        /// <summary>
        /// Xor
        /// </summary>
        public const string XOR = FormatterNamePrefix + "XOR";
        /// <summary>
        /// Add
        /// </summary>
        public const string ADD = FormatterNamePrefix + "ADD";
        /// <summary>
        /// Subtract
        /// </summary>
        public const string SUBTRACT = FormatterNamePrefix + "SUBTRACT";
        /// <summary>
        /// Multiply
        /// </summary>
        public const string MULTIPLY = FormatterNamePrefix + "MULTIPLY";
        /// <summary>
        /// Divide
        /// </summary>
        public const string DIVIDE = FormatterNamePrefix + "DIVIDE";
        /// <summary>
        /// Modulo
        /// </summary>
        public const string MODULO = FormatterNamePrefix + "MODULO";
        /// <summary>
        /// Left shift
        /// </summary>
        public const string LEFT_SHIFT = FormatterNamePrefix + "LEFT_SHIFT";
        /// <summary>
        /// Right shift
        /// </summary>
        public const string RIGHT_SHIFT = FormatterNamePrefix + "RIGHT_SHIFT";
        /// <summary>
        /// Trim
        /// </summary>
        public const string TRIM = FormatterNamePrefix + "TRIM";
        /// <summary>
        /// Trim start
        /// </summary>
        public const string TRIM_START = FormatterNamePrefix + "TRIM_START";
        /// <summary>
        /// Trim end
        /// </summary>
        public const string TRIM_END = FormatterNamePrefix + "TRIM_END";
        /// <summary>
        /// String concat
        /// </summary>
        public const string STRING_CONCAT = FormatterNamePrefix + "STRING_CONCAT";
        /// <summary>
        /// DateTime date
        /// </summary>
        public const string DATE_TIME_DATE = FormatterNamePrefix + "DATE_TIME_DATE";
        /// <summary>
        /// DateTime year
        /// </summary>
        public const string DATE_TIME_YEAR = FormatterNamePrefix + "DATE_TIME_YEAR";
        /// <summary>
        /// DateTime month
        /// </summary>
        public const string DATE_TIME_MONTH = FormatterNamePrefix + "DATE_TIME_MONTH";
        /// <summary>
        /// DateTime day
        /// </summary>
        public const string DATE_TIME_DAY = FormatterNamePrefix + "DATE_TIME_DAY";
        /// <summary>
        /// DateTime day of week
        /// </summary>
        public const string DATE_TIME_DAY_OF_WEEK = FormatterNamePrefix + "DATE_TIME_DAY_OF_WEEK";
        /// <summary>
        /// DateTime day of year
        /// </summary>
        public const string DATE_TIME_DAY_OF_YEAR = FormatterNamePrefix + "DATE_TIME_DAY_OF_YEAR";
        /// <summary>
        /// DateTime hour
        /// </summary>
        public const string DATE_TIME_HOUR = FormatterNamePrefix + "DATE_TIME_HOUR";
        /// <summary>
        /// DateTime Minute
        /// </summary>
        public const string DATE_TIME_MINUTE = FormatterNamePrefix + "DATE_TIME_MINUTE";
        /// <summary>
        /// DateTime Second
        /// </summary>
        public const string DATE_TIME_SECOND = FormatterNamePrefix + "DATE_TIME_SECOND";
        /// <summary>
        /// DateTime Millisecond
        /// </summary>
        public const string DATE_TIME_MILLISECOND = FormatterNamePrefix + "DATE_TIME_MILLISECOND";
        /// <summary>
        /// DateTime time of day
        /// </summary>
        public const string DATE_TIME_TIME_OF_DAY = FormatterNamePrefix + "DATE_TIME_TIME_OF_DAY";
        /// <summary>
        /// DateTime utc
        /// </summary>
        public const string DATE_TIME_UTC = FormatterNamePrefix + "DATE_TIME_UTC";
        /// <summary>
        /// To date format string
        /// </summary>
        public const string DATE_TIME_FORMAT_STRING = FormatterNamePrefix + "DATE_TIME_FORMAT_STRING";
        /// <summary>
        /// Date time string （yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public const string DATE_TIME_STRING = FormatterNamePrefix + "DATE_TIME_STRING";
        /// <summary>
        /// Date time with millisecond string (yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        public const string DATE_TIME_WITH_MILLISECOND_STRING = FormatterNamePrefix + "DATE_TIME_WITH_MILLISECOND_STRING";
        /// <summary>
        /// Date string (yyyy-MM-dd)
        /// </summary>
        public const string DATE_STRING = FormatterNamePrefix + "DATE_STRING";
        /// <summary>
        /// Date string (MM/dd/yyyy)
        /// </summary>
        public const string US_DATE_STRING = FormatterNamePrefix + "US_DATE_STRING";
        /// <summary>
        /// Date string (yyyy/MM/dd)
        /// </summary>
        public const string JAPAN_DATE_STRING = FormatterNamePrefix + "JAPAN_DATE_STRING";
        /// <summary>
        /// TimeSpan days
        /// </summary>
        public const string TIME_SPAN_DAYS = FormatterNamePrefix + "TIME_SPAN_DAYS";
        /// <summary>
        /// TimeSpan days
        /// </summary>
        public const string TIME_SPAN_TOTAL_DAYS = FormatterNamePrefix + "TIME_SPAN_TOTAL_DAYS";
        /// <summary>
        /// TimeSpan Hours
        /// </summary>
        public const string TIME_SPAN_HOURS = FormatterNamePrefix + "TIME_SPAN_HOURS";
        /// <summary>
        /// TimeSpan Hours
        /// </summary>
        public const string TIME_SPAN_TOTAL_HOURS = FormatterNamePrefix + "TIME_SPAN_TOTAL_HOURS";
        /// <summary>
        /// TimeSpan Minutes
        /// </summary>
        public const string TIME_SPAN_MINUTES = FormatterNamePrefix + "TIME_SPAN_MINUTES";
        /// <summary>
        /// TimeSpan Minutes
        /// </summary>
        public const string TIME_SPAN_TOTAL_MINUTES = FormatterNamePrefix + "TIME_SPAN_TOTAL_MINUTES";
        /// <summary>
        /// TimeSpan Seconds
        /// </summary>
        public const string TIME_SPAN_SECONDS = FormatterNamePrefix + "TIME_SPAN_SECONDS";
        /// <summary>
        /// TimeSpan Seconds
        /// </summary>
        public const string TIME_SPAN_TOTAL_SECONDS = FormatterNamePrefix + "TIME_SPAN_TOTAL_SECONDS";
        /// <summary>
        /// TimeSpan Milliseconds
        /// </summary>
        public const string TIME_SPAN_MILLISECONDS = FormatterNamePrefix + "TIME_SPAN_MILLISECONDS";
        /// <summary>
        /// TimeSpan Milliseconds
        /// </summary>
        public const string TIME_SPAN_TOTAL_MILLISECONDS = FormatterNamePrefix + "TIME_SPAN_TOTAL_MILLISECONDS";
        /// <summary>
        /// Distinct
        /// </summary>
        public const string DISTINCT = FormatterNamePrefix + "DISTINCT";
        /// <summary>
        /// Is null
        /// </summary>
        public const string IS_NULL = FormatterNamePrefix + "IS_NULL";
        /// <summary>
        /// Not null
        /// </summary>
        public const string NOT_NULL = FormatterNamePrefix + "NOT_NULL";
        /// <summary>
        /// To string
        /// </summary>
        public const string TO_STRING = FormatterNamePrefix + "TO_STRING";
        /// <summary>
        /// to lower
        /// </summary>
        public const string TO_LOWER = FormatterNamePrefix + "TO_LOWER";
        /// <summary>
        /// to upper
        /// </summary>
        public const string TO_UPPER = FormatterNamePrefix + "TO_UPPER";
        /// <summary>
        /// Sub string
        /// </summary>
        public const string SUB_STRING = FormatterNamePrefix + "SUB_STRING";
        /// <summary>
        /// String replace
        /// </summary>
        public const string STRING_REPLACE = FormatterNamePrefix + "STRING_REPLACE";
        /// <summary>
        /// DateTime add day
        /// </summary>
        public const string DATE_TIME_ADD_DAY = FormatterNamePrefix + "DATE_TIME_ADD_DAY";
        /// <summary>
        /// DateTime add month
        /// </summary>
        public const string DATE_TIME_ADD_MONTH = FormatterNamePrefix + "DATE_TIME_ADD_MONTH";
        /// <summary>
        /// DateTime add year
        /// </summary>
        public const string DATE_TIME_ADD_YEAR = FormatterNamePrefix + "DATE_TIME_ADD_YEAR";
        /// <summary>
        /// DateTime add hour
        /// </summary>
        public const string DATE_TIME_ADD_HOUR = FormatterNamePrefix + "DATE_TIME_ADD_HOUR";
        /// <summary>
        /// DateTime add minute
        /// </summary>
        public const string DATE_TIME_ADD_MINUTE = FormatterNamePrefix + "DATE_TIME_ADD_MINUTE";
        /// <summary>
        /// DateTime add second
        /// </summary>
        public const string DATE_TIME_ADD_SECOND = FormatterNamePrefix + "DATE_TIME_ADD_SECOND";
        /// <summary>
        /// DateTime add Millisecond
        /// </summary>
        public const string DATE_TIME_ADD_MILLISECOND = FormatterNamePrefix + "DATE_TIME_ADD_MILLISECOND";
        /// <summary>
        /// Convert Int32
        /// </summary>
        public const string CONVERT_TO_INT = FormatterNamePrefix + "CONVERT_TO_INT";
        /// <summary>
        /// Convert bool
        /// </summary>
        public const string CONVERT_TO_BOOLEAN = FormatterNamePrefix + "CONVERT_TO_BOOLEAN";
        /// <summary>
        /// Convert byte
        /// </summary>
        public const string CONVERT_TO_BYTE = FormatterNamePrefix + "CONVERT_TO_BYTE";
        /// <summary>
        /// Convert char
        /// </summary>
        public const string CONVERT_TO_CHAR = FormatterNamePrefix + "CONVERT_TO_CHAR";
        /// <summary>
        /// Convert date time
        /// </summary>
        public const string CONVERT_TO_DATE_TIME = FormatterNamePrefix + "CONVERT_TO_DATE_TIME";
        /// <summary>
        /// Convert decimal
        /// </summary>
        public const string CONVERT_TO_DECIMAL = FormatterNamePrefix + "CONVERT_TO_DECIMAL";
        /// <summary>
        /// Convert double
        /// </summary>
        public const string CONVERT_TO_DOUBLE = FormatterNamePrefix + "CONVERT_TO_DOUBLE";
        /// <summary>
        /// Convert int16
        /// </summary>
        public const string CONVERT_TO_INT_16 = FormatterNamePrefix + "CONVERT_TO_INT_16";
        /// <summary>
        /// Convert int64
        /// </summary>
        public const string CONVERT_TO_INT_64 = FormatterNamePrefix + "CONVERT_TO_INT_64";
        /// <summary>
        /// Convert sbyte
        /// </summary>
        public const string CONVERT_TO_SBYTE = FormatterNamePrefix + "CONVERT_TO_SBYTE";
        /// <summary>
        /// Convert single
        /// </summary>
        public const string CONVERT_TO_SINGLE = FormatterNamePrefix + "CONVERT_TO_SINGLE";
        /// <summary>
        /// Convert uint16
        /// </summary>
        public const string CONVERT_TO_UINT_16 = FormatterNamePrefix + "CONVERT_TO_UINT_16";
        /// <summary>
        /// Convert uint32
        /// </summary>
        public const string CONVERT_TO_UINT_32 = FormatterNamePrefix + "CONVERT_TO_UINT_32";
        /// <summary>
        /// Convert uint64
        /// </summary>
        public const string CONVERT_TO_UINT_64 = FormatterNamePrefix + "CONVERT_TO_UINT_64";
        /// <summary>
        /// Math round
        /// </summary>
        public const string MATH_ROUND = FormatterNamePrefix + "MATH_ROUND";
        /// <summary>
        /// Math abs
        /// </summary>
        public const string MATH_ABS = FormatterNamePrefix + "MATH_ABS";
        /// <summary>
        /// Math ceiling
        /// </summary>
        public const string MATH_CEILING = FormatterNamePrefix + "MATH_CEILING";
        /// <summary>
        /// Math floor
        /// </summary>
        public const string MATH_FLOOR = FormatterNamePrefix + "MATH_FLOOR";
        /// <summary>
        /// Math truncate
        /// </summary>
        public const string MATH_TRUNCATE = FormatterNamePrefix + "MATH_TRUNCATE";
        /// <summary>
        /// Math sign
        /// </summary>
        public const string MATH_SIGN = FormatterNamePrefix + "MATH_SIGN";
        /// <summary>
        /// Math Pow
        /// </summary>
        public const string MATH_POW = FormatterNamePrefix + "MATH_POW";
        /// <summary>
        /// Math Sqrt
        /// </summary>
        public const string MATH_SQRT = FormatterNamePrefix + "MATH_SQRT";
        /// <summary>
        /// Math Exp
        /// </summary>
        public const string MATH_EXP = FormatterNamePrefix + "MATH_EXP";
        /// <summary>
        /// Math log
        /// </summary>
        public const string MATH_LOG = FormatterNamePrefix + "MATH_LOG";
        /// <summary>
        /// Math cos
        /// </summary>
        public const string MATH_COS = FormatterNamePrefix + "MATH_COS";
        /// <summary>
        /// Math sin
        /// </summary>
        public const string MATH_SIN = FormatterNamePrefix + "MATH_SIN";
        /// <summary>
        /// Math tan
        /// </summary>
        public const string MATH_TAN = FormatterNamePrefix + "MATH_TAN";
        /// <summary>
        /// Math acos
        /// </summary>
        public const string MATH_ACOS = FormatterNamePrefix + "MATH_ACOS";
        /// <summary>
        /// Math asin
        /// </summary>
        public const string MATH_ASIN = FormatterNamePrefix + "MATH_ASIN";
        /// <summary>
        /// Math atan
        /// </summary>
        public const string MATH_ATAN = FormatterNamePrefix + "MATH_ATAN";
        /// <summary>
        /// Math atan2
        /// </summary>
        public const string MATH_ATAN2 = FormatterNamePrefix + "MATH_ATAN2";
        /// <summary>
        /// String padleft
        /// </summary>
        public const string STRING_PAD_LEFT = FormatterNamePrefix + "STRING_PAD_LEFT";
        /// <summary>
        /// String padright
        /// </summary>
        public const string STRING_PAD_RIGHT = FormatterNamePrefix + "STRING_PAD_RIGHT";
        /// <summary>
        /// String index of
        /// </summary>
        public const string STRING_INDEX_OF = FormatterNamePrefix + "STRING_INDEX_OF";
        /// <summary>
        /// String index of any
        /// </summary>
        public const string STRING_INDEX_OF_ANY = FormatterNamePrefix + "STRING_INDEX_OF_ANY";
        /// <summary>
        /// String last index of
        /// </summary>
        public const string STRING_LAST_INDEX_OF = FormatterNamePrefix + "STRING_LAST_INDEX_OF";
        /// <summary>
        /// String last index of nay
        /// </summary>
        public const string STRING_LAST_INDEX_OF_ANY = FormatterNamePrefix + "STRING_LAST_INDEX_OF_ANY";
        /// <summary>
        /// Exists
        /// </summary>
        public const string EXISTS = FormatterNamePrefix + "EXISTS";
        /// <summary>
        /// Not exists
        /// </summary>
        public const string NOT_EXISTS = FormatterNamePrefix + "NOT_EXISTS";
    }
}
