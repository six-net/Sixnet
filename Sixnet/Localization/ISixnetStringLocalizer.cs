using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Sixnet.Localization
{
    /// <summary>
    /// Defines sixnet string localizer
    /// </summary>
#pragma warning disable CS3027
    public interface ISixnetStringLocalizer : IStringLocalizer
#pragma warning restore CS3027
    {
        /// <summary>
        /// Gets the string resource with the given name
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        LocalizedString this[CultureInfo culture, string name] { get; }

        /// <summary>
        /// Gets the string resource with the given name
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        LocalizedString this[string cultureName, string name] { get; }

        /// <summary>
        ///  Gets the string resource with the given name and formatted with the supplied arguments
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        LocalizedString this[CultureInfo culture, string name, params object[] arguments] { get; }

        /// <summary>
        ///  Gets the string resource with the given name and formatted with the supplied arguments
        /// </summary>
        /// <param name="cultureName">Culture name</param>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns></returns>
        LocalizedString this[string cultureName, string name, params object[] arguments] { get; }
    }

#pragma warning disable CS3027
    public interface ISixnetStringLocalizer<out T> : ISixnetStringLocalizer
#pragma warning restore CS3027
    {
    }
}
