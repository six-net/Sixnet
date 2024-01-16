using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Localization;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    /// <summary>
    /// Localizer
    /// </summary>
    public static class Localizer
    {
        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Local string</returns>
        public static string GetString(string name)
        {
            return GetSixnetStringLocalizer()[name] ?? name;
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Local string</returns>
        public static string GetString<TResourceSource>(string name)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[name] ?? name;
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString(string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer()[name, arguments] ?? name;
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString<TResourceSource>(string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[name, arguments] ?? name;
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string name)
        {
            return GetSixnetStringLocalizer()[culture, name];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetString<TResourceSource>(CultureInfo culture, string name)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[culture, name];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer()[culture, name, arguments];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString<TResourceSource>(CultureInfo culture, string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[culture, name, arguments];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetString(string cultureName, string name)
        {
            return GetSixnetStringLocalizer()[cultureName, name];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetString<TResourceSource>(string cultureName, string name)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[cultureName, name];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString(string cultureName, string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer()[cultureName, name, arguments];
        }

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        public static string GetString<TResourceSource>(string cultureName, string name, params object[] arguments)
        {
            return GetSixnetStringLocalizer<TResourceSource>()[cultureName, name, arguments];
        }

        /// <summary>
        /// Get a sixnet string localizer
        /// </summary>
        /// <returns></returns>
        static ISixnetStringLocalizer GetSixnetStringLocalizer()
        {
            var localizer = ContainerManager.Resolve<ISixnetStringLocalizer>();
            ThrowHelper.ThrowFrameworkErrorIf(localizer == null, "Get sixnet string localizer failed.");
            return localizer;
        }

        static ISixnetStringLocalizer<TResourceSource> GetSixnetStringLocalizer<TResourceSource>()
        {
            var localizer = ContainerManager.Resolve<ISixnetStringLocalizer<TResourceSource>>();
            ThrowHelper.ThrowFrameworkErrorIf(localizer == null, "Get sixnet string localizer failed.");
            return localizer;
        }
    }
}
