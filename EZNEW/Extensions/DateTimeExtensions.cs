using System.Data.SqlTypes;
using System.Globalization;

namespace System
{
    /// <summary>
    /// Date time extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        internal static readonly CultureInfo chineseCultureInfo = new CultureInfo("zh-CN");

        #region Formatting DateTime to like (yyyy-MM-dd)

        /// <summary>
        /// Formatting DateTime to like (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToDate();
        }

        /// <summary>
        /// Formatting DateTime to like (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Formatting DateTime to chinese format like（yyyy年MM月dd日）

        /// <summary>
        /// Formatting DateTime to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDate();
        }

        /// <summary>
        /// Formatting DateTime to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日");
        }

        #endregion

        #region Formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）

        /// <summary>
        /// Formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDateTime();
        }

        /// <summary>
        /// Formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #endregion

        #region Formatting DateTime to chinese format with week like（yyyy年MM月dd日 星期一）

        /// <summary>
        /// Formatting DateTime to chinese format with week like（yyyy年MM月dd日 星期一）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateWeek(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 dddd", chineseCultureInfo); ;
        }

        /// <summary>
        /// Formatting DateTime to chinese format with week like（yyyy年MM月dd日 星期一）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateWeek(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDateWeek();
        }

        #endregion

        #region Formatting DateTime to like(yyyy-MM-dd HH:mm:ss)

        /// <summary>
        /// Formatting DateTime to like(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToSecond(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Formatting DateTime to like(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToSecond(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToSecond();
        }

        #endregion

        #region Formatting DateTime to like(yyyy-MM-dd HH:mm)

        /// <summary>
        /// Formatting DateTime to like(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToMinute(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// Formatting DateTime to like(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToMinute(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToMinute();
        }

        #endregion

        #region Formatting DateTime to like (yyyy-MM-dd HH)

        /// <summary>
        /// Formatting DateTime to like (yyyy-MM-dd HH)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToHour(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH");
        }

        /// <summary>
        /// Formatting DateTime to like (yyyy-MM-dd HH)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToHour(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToHour();
        }

        #endregion

        #region Calculate date time difference

        /// <summary>
        /// Calculate datetime difference
        /// </summary>
        /// <param name="sourceDateTime">Source datatime</param>
        /// <param name="targetDateTime">Target datetime</param>
        public static TimeSpan CalculateTime(this DateTime sourceDateTime, DateTime targetDateTime)
        {
            DateTime maxDate = targetDateTime;
            DateTime minDate = sourceDateTime;
            if (sourceDateTime > targetDateTime)
            {
                minDate = targetDateTime;
                maxDate = sourceDateTime;
            }
            TimeSpan timeSpan = maxDate - minDate;
            return timeSpan;
        }

        #endregion

        #region Wheather is less sql mininum time

        /// <summary>
        /// Wheather is less sql mininum time
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <returns></returns>
        public static bool LessSqlMinimumTime(this DateTime dateTime)
        {
            SqlDateTime minValue = SqlDateTime.MinValue;
            return dateTime < minValue.Value;
        }

        /// <summary>
        /// Wheather is less sql mininum time
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <returns></returns>
        public static bool LessSqlMinimumTime(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.LessSqlMinimumTime();
            }
            return true;
        }

        #endregion
    }

    /// <summary>
    ///Date time offset extension
    /// </summary>
    public static class DateTimeOffsetExtension
    {
        #region Formatting DateTimeOffset to like (yyyy-MM-dd)

        /// <summary>
        /// Formatting DateTimeOffset to like (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToDate(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToDate();
        }

        /// <summary>
        /// Formatting DateTimeOffset to like (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToDate(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日）

        /// <summary>
        /// Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDate(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDate();
        }

        /// <summary>
        /// Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDate(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日");
        }

        #endregion

        #region Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日 HH:mm:ss）

        /// <summary>
        /// Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateTime(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDateTime();
        }

        /// <summary>
        /// Formatting DateTimeOffset to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateTime(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #endregion

        #region Formatting DateTimeOffset to chinese format with week like（yyyy年MM月dd日 星期一）

        /// <summary>
        /// Formatting DateTimeOffset to chinese format with week like（yyyy年MM月dd日 星期一）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateWeek(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 dddd", DateTimeExtensions.chineseCultureInfo); ;
        }

        /// <summary>
        /// Formatting DateTimeOffset to chinese format with week like（yyyy年MM月dd日 星期一）
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string ToChineseDateWeek(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToChineseDateWeek();
        }

        #endregion

        #region Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm:ss)

        /// <summary>
        /// Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToSecond(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToSecond(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToSecond();
        }

        #endregion

        #region Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm)

        /// <summary>
        /// Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToMinute(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// Formatting DateTimeOffset to like(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToMinute(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToMinute();
        }

        #endregion

        #region Formatting DateTimeOffset to like (yyyy-MM-dd HH)

        /// <summary>
        /// Formatting DateTimeOffset to like (yyyy-MM-dd HH)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToHour(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH");
        }

        /// <summary>
        /// Formatting DateTimeOffset to like (yyyy-MM-dd HH)
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return formatted value</returns>
        public static string FormatToHour(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.FormatToHour();
        }

        #endregion

        #region Calculate date time difference

        /// <summary>
        /// Calculate datetime difference
        /// </summary>
        /// <param name="sourceDateTime">Source datatime</param>
        /// <param name="targetDateTime">Target datetime</param>
        public static TimeSpan CalculateTime(this DateTimeOffset sourceDateTime, DateTimeOffset targetDateTime)
        {
            DateTimeOffset maxDate = targetDateTime;
            DateTimeOffset minDate = sourceDateTime;
            if (sourceDateTime > targetDateTime)
            {
                minDate = targetDateTime;
                maxDate = sourceDateTime;
            }
            TimeSpan timeSpan = maxDate - minDate;
            return timeSpan;
        }

        #endregion

        #region Wheather is less sql mininum time

        /// <summary>
        /// Wheather is less sql mininum time
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return wheather is less sql mininum time</returns>
        public static bool LessSqlMinimumTime(this DateTimeOffset dateTime)
        {
            SqlDateTime minValue = SqlDateTime.MinValue;
            return dateTime < minValue.Value;
        }

        /// <summary>
        /// Wheather is less sql mininum time
        /// </summary>
        /// <param name="dateTime">Datetime value</param>
        /// <returns>Return wheather is less sql mininum time</returns>
        public static bool LessSqlMinimumTime(this DateTimeOffset? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.LessSqlMinimumTime();
            }
            return true;
        }

        #endregion
    }
}
