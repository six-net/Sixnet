// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Queryable
{
    public static class SixnetFunc
    {
        /// <summary>
        /// Max func
        /// </summary>
        /// <param name="value">Original value</param>
        /// <returns></returns>
        public static T Max<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Min func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Min<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Avg func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Avg<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Count func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Count<T>(T value)
        {
            return 0;
        }

        /// <summary>
        /// Sum func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Sum<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Json value func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonValue<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Json object func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonObject<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Is null
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool IsNull(object value)
        {
            return true;
        }

        /// <summary>
        /// Not null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NotNull(object value)
        {
            return true;
        }

        /// <summary>
        /// Distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Distinct<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString<T>(T value, int length = 200)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time string （yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeString<T>(T value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date time with millisecond string (yyyy-MM-dd HH:mm:ss.ms)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeWithMillisecondString<T>(T value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To date string (yyyy-MM-dd)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateString<T>(T value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To US date string (MM/dd/yyyy)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUSDateString<T>(T value)
        {
            return string.Empty;
        }

        /// <summary>
        /// To US date string (yyyy/MM/dd)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJapanDateString<T>(T value)
        {
            return string.Empty;
        }
    }
}
