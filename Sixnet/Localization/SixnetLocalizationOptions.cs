// "Company © 2025. All rights reserved."

using Microsoft.Extensions.Localization;

namespace Sixnet.Localization
{
    /// <summary>
    /// Sixnet localization options
    /// </summary>
#pragma warning disable CS3009
    public class SixnetLocalizationOptions : LocalizationOptions
#pragma warning restore CS3009
    {
        /// <summary>
        /// Json resource path
        /// </summary>
        public string JsonResourcePath { get; set; } = "Resources";

        /// <summary>
        /// Whether Auto localize throwed message
        /// </summary>
        public bool AutoLocalizeThrowMessage { get; set; } = true;
    }
}
