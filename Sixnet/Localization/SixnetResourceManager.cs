// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

using Sixnet.DependencyInjection;
using Sixnet.Localization.Database;
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
        readonly SixnetDatabaseResourceManager _databaseManager;

        public SixnetResourceManager(ResourcePrefix resourcePrefix, Assembly assembly)
        {
            var localizationOptions = SixnetContainer.GetOptions<SixnetLocalizationOptions>();
            var resourceSource = localizationOptions.ResourceSource;

            if ((resourceSource & SixnetLocalizationResourceSource.Resx) == SixnetLocalizationResourceSource.Resx)
            {
                _resxManager = new SixnetResxManager(resourcePrefix, assembly);
            }
            if ((resourceSource & SixnetLocalizationResourceSource.JSON) == SixnetLocalizationResourceSource.JSON)
            {
                _jsonManager = new SixnetJsonManager(resourcePrefix);
            }
            if ((resourceSource & SixnetLocalizationResourceSource.Database) == SixnetLocalizationResourceSource.Database)
            {
                _databaseManager = new SixnetDatabaseResourceManager(resourcePrefix);
            }
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

            if (_resxManager != null)
            {
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
            }

            if (_jsonManager != null)
            {
                var jsonResourceSet = _jsonManager.GetResourceSet(culture, tryParents);
                if (!jsonResourceSet.IsNullOrEmpty())
                {
                    foreach (var jsonResourceItem in jsonResourceSet)
                    {
                        resourceSet.TryAdd(jsonResourceItem.Key, jsonResourceItem.Value);
                    }
                }
            }

            if (_databaseManager != null)
            {
                var databaseResourceSet = _databaseManager.GetResourceSet(culture, tryParents);
                if (!databaseResourceSet.IsNullOrEmpty())
                {
                    foreach (var databaseResourceItem in databaseResourceSet)
                    {
                        resourceSet.TryAdd(databaseResourceItem.Key, databaseResourceItem.Value);
                    }
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
