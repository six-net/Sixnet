// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Queryable
{
    public static class SixnetFieldExtensions
    {
        /// <summary>
        /// Distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DbDistinct<T>(this T value)
        {
            return value;
        }

        /// <summary>
        /// Max
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DbMax<T>(this T value)
        {
            return value;
        }

        /// <summary>
        /// Min
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DbMin<T>(this T value)
        {
            return value;
        }

        /// <summary>
        /// Avg
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DbAvg<T>(this T value)
        {
            return value;
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long DbCount<T>(this T value)
        {
            return 0;
        }

        /// <summary>
        /// Sum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DbSum<T>(this T value)
        {
            return value;
        }

        /// <summary>
        /// Json value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DbJsonValue<T>(this string value, string path)
        {
            return default;
        }

        /// <summary>
        /// Json value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DbJsonObject<T>(this string value, string path)
        {
            return default;
        }

        /// <summary>
        /// IsNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool DbIsNull<T>(this T value)
        {
            return true;
        }

        /// <summary>
        /// NotNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool DbNotNull<T>(this T value)
        {
            return true;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string DbToString<T>(this T value, int length = 200)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeString(this DateTime value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeString(this DateTime? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeString(this DateTimeOffset value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeString(this DateTimeOffset? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time with millisecond string (yyyy-MM-dd HH:mm:ss.ms)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeWithMillisecondString(this DateTime value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time with millisecond string (yyyy-MM-dd HH:mm:ss.ms)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeWithMillisecondString(this DateTime? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time with millisecond string (yyyy-MM-dd HH:mm:ss.ms)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeWithMillisecondString(this DateTimeOffset value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time with millisecond string (yyyy-MM-dd HH:mm:ss.ms)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateTimeWithMillisecondString(this DateTimeOffset? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy-MM-dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateString(this DateTime value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy-MM-dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateString(this DateTime? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy-MM-dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateString(this DateTimeOffset value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy-MM-dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToDateString(this DateTimeOffset? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (MM/dd/yyyy)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToUSDateString(this DateTime value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (MM/dd/yyyy)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToUSDateString(this DateTime? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (MM/dd/yyyy)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToUSDateString(this DateTimeOffset value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (MM/dd/yyyy)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToUSDateString(this DateTimeOffset? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy/MM/dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToJapanDateString(this DateTime value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy/MM/dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToJapanDateString(this DateTime? value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy/MM/dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToJapanDateString(this DateTimeOffset value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy/MM/dd)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbToJapanDateString(this DateTimeOffset? value)
        {
            return string.Empty;
        }
    }
}
