// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;
using System.Globalization;

using Sixnet.DependencyInjection;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Localization.Json;
using Sixnet.Model.Paging;
using Sixnet.Threading.Locking;

namespace Sixnet.Localization.Database
{
    internal class SixnetDatabaseResourceManager : ISixnetResourceManager
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

        public SixnetDatabaseResourceManager(ResourcePrefix resourcePrefix)
        {
            _resourcePrefix = resourcePrefix;
            _resourceBaseName = _resourcePrefix.JsonBaseName?.Trim('.');
            _resourcePath = _resourcePrefix.JsonResourcePath?.Trim('.');
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
            if (_resourcesCache.ContainsKey(culture.Name) || culture.Name == CultureInfo.InvariantCulture.Name)
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
                var resources = new ConcurrentDictionary<string, string>();

                var resourceFilter = new SixnetPagingFilter()
                {
                    Page = 1,
                    PageSize = 1000
                };
                var repository = SixnetContainer.GetRepository<SixnetLocalizationEntity>();
                SixnetPagingInfo<SixnetLocalizationEntity> resourcePaging = null;
                do
                {
                    resourcePaging = repository.AsQueryable()
                        .OrderBy(c => c.Code)
                        .SplitTable(culture.Name)
                        .ToPaging(resourceFilter);
                    if (resourcePaging != null && !resourcePaging.Items.IsNullOrEmpty())
                    {
                        foreach (var item in resourcePaging.Items)
                        {
                            resources.TryAdd(item.Code, item.Text);
                        }
                        resourceFilter.Page++;
                    }

                } while (resourcePaging != null && !resourcePaging.Items.IsNullOrEmpty());

                _resourcesCache.TryAdd(culture.Name, resources);
            }
            catch (Exception ex)
            {
                throw;
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
}
