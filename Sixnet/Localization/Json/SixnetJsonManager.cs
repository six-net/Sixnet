using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using Microsoft.Extensions.Configuration;
using Sixnet.App;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Threading.Locking;

namespace Sixnet.Localization.Json
{
    /// <summary>
    /// Defines json resource manager
    /// </summary>
    public class SixnetJsonManager : ISixnetResourceManager
    {
        /// <summary>
        /// Resources cache
        /// Key: culture name
        /// Value: key => code, value => local string
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new();
        private readonly string _resourcesPath;
        private readonly string _resourceName;

        public SixnetJsonManager(string resourcesPath, string resourceName = null)
        {
            _resourcesPath = resourcesPath;
            _resourceName = resourceName;
        }

        /// <summary>
        /// Gets the base name
        /// </summary>
        public string BaseName => _resourceName;

        /// <summary>
        /// Get resource set
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="tryParents">Whether try parents</param>
        /// <returns></returns>
        public virtual ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
        {
            if (tryParents)
            {
                var allResources = new ConcurrentDictionary<string, string>();
                do
                {
                    LoadResourceSet(culture);
                    if (_resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources))
                    {
                        foreach (var entry in resources)
                        {
                            allResources.TryAdd(entry.Key, entry.Value);
                        }
                    }
                    culture = culture.Parent;
                } while (culture != CultureInfo.InvariantCulture);
                return allResources;
            }
            else
            {
                LoadResourceSet(culture);
                _resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources);
                return resources;
            }
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public virtual string GetString(string name)
        {
            return GetString(name, null);
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="culture">Culture</param>
        /// <returns></returns>
        public virtual string GetString(string name, CultureInfo culture)
        {
            ThrowHelper.ThrowArgNullIf(name == null, nameof(name));

            culture ??= CultureInfo.CurrentUICulture;
            do
            {
                LoadResourceSet(culture);
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    if (_resourcesCache[culture.Name].TryGetValue(name, out string value))
                    {
                        return value;
                    }
                }
                culture = culture.Parent;
            } while (culture != culture.Parent);
            return null;
        }

        /// <summary>
        /// Try load resource set
        /// </summary>
        /// <param name="culture"></param>
        private void LoadResourceSet(CultureInfo culture)
        {
            if (_resourcesCache.ContainsKey(culture.Name))
            {
                return;
            }
            var cultureLock = LockManager.GetLoadLocalizationStringLock(culture);
            try
            {
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    return;
                }
                var resources = new ConcurrentDictionary<string, string>();
                var resourceRootPath = ApplicationManager.GetRootPath();
                if (!string.IsNullOrWhiteSpace(_resourcesPath))
                {
                    resourceRootPath = Path.Combine(resourceRootPath, _resourcesPath);
                }
                // folders
                var cultureFolder = Path.Combine(resourceRootPath, culture.Name);
                if (Directory.Exists(cultureFolder))
                {
                    var cultureFolderFiles = Directory.GetFiles(cultureFolder, "*.json", SearchOption.AllDirectories);
                    if (!cultureFolderFiles.IsNullOrEmpty())
                    {
                        foreach (var file in cultureFolderFiles)
                        {
                            var fileResources = new ConfigurationBuilder()
                                .AddJsonFile(file, optional: true, reloadOnChange: false)
                                .Build().AsEnumerable();

                            foreach (var fileResource in fileResources)
                            {
                                resources.TryAdd(fileResource.Key, fileResource.Value);
                            }
                        }
                    }
                }

                //files
                var cultureFiles = Directory.GetFiles(resourceRootPath, $"{culture.Name}-?.json|{culture.Name}.json");
                if (!cultureFiles.IsNullOrEmpty())
                {
                    foreach (var file in cultureFiles)
                    {
                        var fileResources = new ConfigurationBuilder()
                            .AddJsonFile(file, optional: true, reloadOnChange: false)
                            .Build().AsEnumerable();
                        foreach (var fileResource in fileResources)
                        {
                            resources.TryAdd(fileResource.Key, fileResource.Value);
                        }
                    }
                }
                _resourcesCache.TryAdd(culture.Name, resources);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cultureLock?.Release();
            }
        }

        /// <summary>
        /// Get resource name
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetResourceName(CultureInfo culture)
        {
            var resourceStreamName = BaseName;
            if (!string.IsNullOrEmpty(culture.Name))
            {
                resourceStreamName += "." + culture.Name;
            }
            return resourceStreamName;
        }
    }
}
