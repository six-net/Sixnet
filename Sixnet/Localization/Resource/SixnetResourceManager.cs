using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Sixnet.Localization.Resource
{
    public class SixnetResourceManager : ISixnetResourceManager
    {
        private readonly ResourceManager _resourceManager;

        public SixnetResourceManager(string baseName, Assembly assembly)
        {
            _resourceManager = new ResourceManager(baseName, assembly);
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
