using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Localization.Internal;
using Microsoft.Extensions.Logging;

namespace Sixnet.Localization
{
    public class SixnetResourceStringLocalizer : ISixnetStringLocalizer
    {
        private readonly ConcurrentDictionary<string, object> _missingManifestCache = new();
        private readonly IResourceNamesCache _resourceNamesCache;
        private readonly ISixnetResourceManager _resourceManager;
        private readonly ISixnetResourceStringProvider _resourceStringProvider;
        private readonly string _resourceBaseName;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="SixnetResourceStringLocalizer"/>.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> to read strings from.</param>
        /// <param name="resourceAssembly">The <see cref="Assembly"/> that contains the strings as embedded resources.</param>
        /// <param name="baseName">The base name of the embedded resource that contains the strings.</param>
        /// <param name="resourceNamesCache">Cache of the list of strings for a given resource assembly name.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public SixnetResourceStringLocalizer(
            ISixnetResourceManager resourceManager,
            Assembly resourceAssembly,
            string baseName,
            IResourceNamesCache resourceNamesCache,
            ILogger logger)
            : this(
                resourceManager,
                new SixnetAssemblyWrapper(resourceAssembly),
                baseName,
                resourceNamesCache,
                logger)
        {
        }

        /// <summary>
        /// Intended for testing purposes only.
        /// </summary>
        internal SixnetResourceStringLocalizer(
            ISixnetResourceManager resourceManager,
            SixnetAssemblyWrapper resourceAssemblyWrapper,
            string baseName,
            IResourceNamesCache resourceNamesCache,
            ILogger logger)
            : this(
                  resourceManager,
                  new SixnetResourceStringProvider(
                      resourceNamesCache,
                      resourceManager,
                      resourceAssemblyWrapper.Assembly,
                      baseName),
                  baseName,
                  resourceNamesCache,
                  logger)
        {
        }

        /// <summary>
        /// Intended for testing purposes only.
        /// </summary>
        internal SixnetResourceStringLocalizer(
            ISixnetResourceManager resourceManager,
            ISixnetResourceStringProvider resourceStringProvider,
            string baseName,
            IResourceNamesCache resourceNamesCache,
            ILogger logger)
        {
            _resourceStringProvider = resourceStringProvider ?? throw new ArgumentNullException(nameof(resourceStringProvider));
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            _resourceBaseName = baseName ?? throw new ArgumentNullException(nameof(baseName));
            _resourceNamesCache = resourceNamesCache ?? throw new ArgumentNullException(nameof(resourceNamesCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual LocalizedString this[string name]
        {
            get
            {
                var value = GetStringSafely(name, null);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetStringSafely(name, null);
                var value = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _resourceBaseName);
            }
        }

        public LocalizedString this[CultureInfo culture, string name]
        {
            get
            {
                var value = GetStringSafely(name, culture);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
            }
        }

        public LocalizedString this[string cultureName, string name]
        {
            get
            {
                var value = GetStringSafely(name, new CultureInfo(cultureName));
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
            }
        }

        public LocalizedString this[CultureInfo culture, string name, params object[] arguments]
        {
            get
            {
                var format = GetStringSafely(name, culture);
                var value = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _resourceBaseName);
            }
        }

        public LocalizedString this[string cultureName, string name, params object[] arguments]
        {
            get
            {
                var format = GetStringSafely(name, new CultureInfo(cultureName));
                var value = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _resourceBaseName);
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

        /// <summary>
        /// Returns all strings in the specified culture.
        /// </summary>
        /// <param name="includeParentCultures">Whether to include parent cultures in the search for a resource.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to get strings for.</param>
        /// <returns>The strings.</returns>
        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var resourceNames = includeParentCultures
                ? GetResourceNamesFromCultureHierarchy(culture)
                : _resourceStringProvider.GetAllResourceStrings(culture, true);

            foreach (var name in resourceNames ?? Enumerable.Empty<string>())
            {
                var value = GetStringSafely(name, culture);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
            }
        }

        /// <summary>
        /// Gets a resource string from a <see cref="ResourceManager"/> and returns <c>null</c> instead of
        /// throwing exceptions if a match isn't found.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to get the string for.</param>
        /// <returns>The resource string, or <c>null</c> if none was found.</returns>
        protected string GetStringSafely(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var keyCulture = culture ?? CultureInfo.CurrentUICulture;
            _logger.SearchedLocation(name, _resourceBaseName, keyCulture);

            var cacheKey = $"name={name}&culture={keyCulture.Name}";
            if (_missingManifestCache.ContainsKey(cacheKey))
            {
                return null;
            }

            try
            {
                return _resourceManager.GetString(name, culture);
            }
            catch (MissingManifestResourceException)
            {
                _missingManifestCache.TryAdd(cacheKey, null);
                return null;
            }
        }

        private IEnumerable<string> GetResourceNamesFromCultureHierarchy(CultureInfo startingCulture)
        {
            var currentCulture = startingCulture;
            var resourceNames = new HashSet<string>();

            var hasAnyCultures = false;

            while (true)
            {

                var cultureResourceNames = _resourceStringProvider.GetAllResourceStrings(currentCulture, false);

                if (cultureResourceNames != null)
                {
                    foreach (var resourceName in cultureResourceNames)
                    {
                        resourceNames.Add(resourceName);
                    }
                    hasAnyCultures = true;
                }

                if (currentCulture == currentCulture.Parent)
                {
                    // currentCulture begat currentCulture, probably time to leave
                    break;
                }

                currentCulture = currentCulture.Parent;
            }

            if (!hasAnyCultures)
            {
                throw new MissingManifestResourceException("No manifests exist for the current culture");
            }

            return resourceNames;
        }
    }
}
