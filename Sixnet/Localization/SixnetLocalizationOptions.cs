using System;
using System.Collections.Generic;
using System.Text;
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
        /// Gets or sets the localization string source
        /// </summary>
        public LocalizationStringSource StringSource { get; set; } = LocalizationStringSource.Resource;
    }
}
