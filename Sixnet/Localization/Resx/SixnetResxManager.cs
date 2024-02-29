using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Sixnet.Localization.Resx
{
    public class SixnetResxManager : ISixnetResourceManager
    {
        private readonly ResourceManager _resourceManager;
        readonly ResourcePrefix _resourcePrefix;

        public SixnetResxManager(ResourcePrefix resourcePrefix, Assembly assembly)
        {
            _resourcePrefix = resourcePrefix;
            _resourceManager = new ResourceManager(_resourcePrefix.ResxBaseName, assembly);
        }

        public string BaseName => _resourceManager.BaseName;

        public string GetResourceName(CultureInfo culture)
        {
            var resourceStreamName = _resourceManager.BaseName;
            if (!string.IsNullOrEmpty(culture.Name))
            {
                resourceStreamName += "." + culture.Name;
            }
            resourceStreamName += ".resources";
            return resourceStreamName;
        }

        public ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
        {
            var resourceSet = _resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: tryParents);
            var allResources = new ConcurrentDictionary<string, string>();
            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Key is string key && entry.Value is string value)
                {
                    allResources.TryAdd(key, value);
                }
            }
            return allResources;
        }

        public string GetString(string name)
        {
            return GetString(name, null);
        }

        public string GetString(string name, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentUICulture;
            return _resourceManager.GetString(name, culture);
        }
    }
}
