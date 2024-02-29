using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using Sixnet.App;
using Sixnet.Exceptions;
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
        ResourcePrefix _resourcePrefix;
        readonly string _resourcePath;
        readonly string _resourceBaseName;

        public SixnetJsonManager(ResourcePrefix resourcePrefix)
        {
            _resourcePrefix = resourcePrefix;
            _resourceBaseName = _resourcePrefix.JsonBaseName?.Trim('.');
            _resourcePath = _resourcePrefix.ResourcePath?.Trim('.');
        }

        /// <summary>
        /// Gets the base name
        /// </summary>
        public string BaseName => _resourcePrefix.ResxBaseName;

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
            SixnetDirectThrower.ThrowArgNullIf(name == null, nameof(name));

            var currentCulture = culture ?? CultureInfo.CurrentUICulture;
            do
            {
                LoadResourceSet(currentCulture);
                if (_resourcesCache.ContainsKey(currentCulture.Name))
                {
                    if (_resourcesCache[currentCulture.Name].TryGetValue(name, out string value))
                    {
                        return value;
                    }
                }
                currentCulture = currentCulture.Parent;
            } while (currentCulture != currentCulture.Parent);

            // share
            var shareManager = SixnetShareJsonManager.GetShareJsonManager(_resourcePath);
            return shareManager?.GetString(name, culture);
        }

        /// <summary>
        /// Try load resource set
        /// </summary>
        /// <param name="culture"></param>
        private void LoadResourceSet(CultureInfo culture)
        {
            if (_resourcesCache.ContainsKey(culture.Name)
                || string.IsNullOrWhiteSpace(_resourceBaseName)
                || culture.Name == CultureInfo.InvariantCulture.Name)
            {
                return;
            }
            var resourceLock = SixnetLocker.GetLoadLocalizationStringLock(_resourceBaseName, culture);
            try
            {
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    return;
                }
                var resourcePath = _resourcePath;
                var resources = new ConcurrentDictionary<string, string>();
                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    resourcePath = SixnetApplication.GetRootPath();
                }
                else
                {
                    resourcePath = Path.Combine(SixnetApplication.GetRootPath(), resourcePath);
                }
                if (!Directory.Exists(resourcePath))
                {
                    return;
                }
                // folders
                var resourceFolder = Path.Combine(resourcePath, _resourceBaseName.Replace('.', Path.AltDirectorySeparatorChar));
                if (Directory.Exists(resourceFolder))
                {
                    var cultureFolderFiles = Directory.GetFiles(resourceFolder, $"{culture.Name}.json", SearchOption.AllDirectories);
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
                var resourceFiles = Directory.GetFiles(resourcePath, $"{_resourceBaseName}.{culture.Name}.json");
                if (!resourceFiles.IsNullOrEmpty())
                {
                    foreach (var file in resourceFiles)
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
                resourceLock?.Release();
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

    internal class SixnetShareJsonManager : ISixnetResourceManager
    {
        static SixnetShareJsonManager _shareJsonManager = null;
        static readonly object _initShareJsonLock = new();
        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new();
        readonly string _resourcesPath;

        /// <summary>
        /// Gets base name
        /// </summary>
        public string BaseName => string.Empty;

        private SixnetShareJsonManager(string resourcePath)
        {
            _resourcesPath = resourcePath;
        }

        /// <summary>
        /// Get resource name
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetResourceName(CultureInfo culture)
        {
            return culture.Name;
        }

        /// <summary>
        /// Get resource set
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="tryParents"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
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
        /// Try load resource set
        /// </summary>
        /// <param name="culture"></param>
        private void LoadResourceSet(CultureInfo culture)
        {
            if (_resourcesCache.ContainsKey(culture.Name)
                || culture.Name == CultureInfo.InvariantCulture.Name)
            {
                return;
            }
            var resourceLock = SixnetLocker.GetLoadLocalizationStringLock(culture);
            try
            {
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    return;
                }
                var resourcePath = _resourcesPath;
                var resources = new ConcurrentDictionary<string, string>();
                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    resourcePath = SixnetApplication.GetRootPath();
                }
                else
                {
                    resourcePath = Path.Combine(SixnetApplication.GetRootPath(), resourcePath);
                }
                if (!Directory.Exists(resourcePath))
                {
                    return;
                }
                // folders
                var cultureFolder = Path.Combine(resourcePath, culture.Name);
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
                var cultureFiles = Directory.GetFiles(resourcePath, $"{culture.Name}.json");
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
                resourceLock?.Release();
            }
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetString(string name)
        {
            return GetString(name, null);
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetString(string name, CultureInfo culture)
        {
            SixnetDirectThrower.ThrowArgNullIf(name == null, nameof(name));

            var currentCulture = culture ?? CultureInfo.CurrentUICulture;
            do
            {
                LoadResourceSet(currentCulture);
                if (_resourcesCache.ContainsKey(currentCulture.Name))
                {
                    if (_resourcesCache[currentCulture.Name].TryGetValue(name, out string value))
                    {
                        return value;
                    }
                }
                currentCulture = currentCulture.Parent;
            } while (currentCulture != currentCulture.Parent);
            return null;
        }

        /// <summary>
        /// Get share json manager
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static SixnetShareJsonManager GetShareJsonManager(string resourcePath)
        {
            if (_shareJsonManager == null)
            {
                lock (_initShareJsonLock)
                {
                    _shareJsonManager ??= new(resourcePath);
                }
            }
            return _shareJsonManager;
        }
    }
}
