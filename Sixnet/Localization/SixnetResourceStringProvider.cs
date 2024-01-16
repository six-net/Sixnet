using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace Sixnet.Localization
{
    internal class SixnetResourceStringProvider : ISixnetResourceStringProvider
    {
        private readonly IResourceNamesCache _resourceNamesCache;
        private readonly ISixnetResourceManager _resourceManager;
        private readonly Assembly _assembly;
        private readonly string _resourceBaseName;

        public SixnetResourceStringProvider(
            IResourceNamesCache resourceCache,
            ISixnetResourceManager resourceManager,
            Assembly assembly,
            string baseName)
        {
            _resourceManager = resourceManager;
            _resourceNamesCache = resourceCache;
            _assembly = assembly;
            _resourceBaseName = baseName;
        }

        private string GetResourceCacheKey(CultureInfo culture)
        {
            var resourceName = _resourceManager.BaseName;
            return $"Culture={culture.Name};resourceName={resourceName};Assembly={_assembly.FullName}";
        }

        private string GetResourceName(CultureInfo culture)
        {
            return _resourceManager.GetResourceName(culture);
        }

        public IList<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing)
        {
            var cacheKey = GetResourceCacheKey(culture);

            return _resourceNamesCache.GetOrAdd(cacheKey, _ =>
            {
                var resourceSet = _resourceManager.GetResourceSet(culture, false);
                if (resourceSet == null)
                {
                    if (throwOnMissing)
                    {
                        throw new MissingManifestResourceException($"The manifest '{GetResourceName(culture)}' was not found");
                    }
                    else
                    {
                        return null;
                    }
                }

                var names = new List<string>();
                foreach (var entry in resourceSet)
                {
                    if (entry.Key is string key)
                    {
                        names.Add(key);
                    }
                }

                return names;
            });
        }
    }
}
