using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Sixnet.Localization.Json;
using Sixnet.Localization.Resx;

namespace Sixnet.Localization
{
    /// <summary>
    /// Sixnet resource manager
    /// </summary>
    public class SixnetResourceManager : ISixnetResourceManager
    {
        readonly SixnetResxManager _resxManager;
        readonly SixnetJsonManager _jsonManager;
        readonly ResourcePrefix _resourcePrefix;

        public SixnetResourceManager(ResourcePrefix resourcePrefix, Assembly assembly)
        {
            _resxManager = new SixnetResxManager(resourcePrefix, assembly);
            _jsonManager = new SixnetJsonManager(resourcePrefix);
            _resourcePrefix = resourcePrefix;
        }

        public string BaseName => _resourcePrefix.ResxBaseName;

        public string GetResourceName(CultureInfo culture)
        {
            return _resxManager.GetResourceName(culture);
        }

        public ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
        {
            var resourceSet = new ConcurrentDictionary<string, string>();
            var resxResourceSet = _resxManager.GetResourceSet(culture, tryParents);
            if (!resxResourceSet.IsNullOrEmpty())
            {
                foreach (var resourceItem in resxResourceSet)
                {
                    foreach (var resxResourceItem in resxResourceSet)
                    {
                        resourceSet.TryAdd(resxResourceItem.Key, resxResourceItem.Value);
                    }
                }
            }
            var jsonResourceSet = _jsonManager.GetResourceSet(culture, tryParents);
            if (!jsonResourceSet.IsNullOrEmpty())
            {
                foreach (var jsonResourceItem in jsonResourceSet)
                {
                    resourceSet.TryAdd(jsonResourceItem.Key, jsonResourceItem.Value);
                }
            }
            return resourceSet;
        }

        public string GetString(string name)
        {
            return GetString(name, null);
        }

        public string GetString(string name, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentUICulture;
            var localString = _jsonManager.GetString(name, culture);
            if (string.IsNullOrEmpty(localString))
            {
                localString = _resxManager.GetString(name, culture);
            }
            return localString;
        }
    }
}
