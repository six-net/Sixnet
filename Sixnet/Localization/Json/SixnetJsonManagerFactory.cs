using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Sixnet.Localization.Json
{
    public class SixnetJsonManagerFactory : ISixnetResourceManagerFactory
    {
        private readonly LocalizationOptions _localizationOptions;

        public SixnetJsonManagerFactory(IOptions<LocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions.Value;
        }

        public ISixnetResourceManager Create(string baseName, Assembly assembly)
        {
            return new SixnetJsonManager(_localizationOptions.ResourcesPath, baseName);
        }
    }
}
