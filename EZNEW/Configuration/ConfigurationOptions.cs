using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Application;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Configuration options
    /// </summary>
    public static class ConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the default configure file match pattern
        /// </summary>
        public static DefaultConfigureFileMatchPattern DefaultConfigureFileMatchPattern { get; set; } = DefaultConfigureFileMatchPattern.Default;

        /// <summary>
        /// Gets or sets the default configure file name match key words
        /// </summary>
        public static List<string> DefaultConfigureFileNameMatchKeyWords { get; set; }

        /// <summary>
        /// Path directory separator
        /// </summary>
        internal static readonly string RegexPathDirectorySeparator = $"{Path.DirectorySeparatorChar}".Replace(@"\", @"\\");

        /// <summary>
        /// Configuration exclude file regex expression
        /// </summary>
        public static Regex ConfigurationExcludeFileRegex => new Regex($@"^{ApplicationManager.ApplicationExecutableDirectory.Replace(@"\", @"\\")}.*(System\.|Microsoft\.|Google\.|IdentityModel\.|AutoMapper\.|MySql\.|Newtonsoft\.|Oracle\.|{RegexPathDirectorySeparator}runtimes{RegexPathDirectorySeparator}|{RegexPathDirectorySeparator}wwwroot{RegexPathDirectorySeparator}|\.resources\.{AdditionalMatchRegexExpression}).*$");

        /// <summary>
        /// Additional match regex expression.
        /// User it with DefaultConfigureFileMatchPattern.Default.
        /// </summary>
        public static string AdditionalMatchRegexExpression = string.Empty;
    }
}
