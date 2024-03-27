using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    /// <summary>
    /// An <see cref="IStringLocalizerFactory"/> that creates instances of <see cref="SixnetResourceStringLocalizer"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="SixnetResourceStringLocalizerFactory"/> offers multiple ways to set the relative path of
    /// resources to be used. They are, in order of precedence:
    /// <see cref="ResourceLocationAttribute"/> -> <see cref="LocalizationOptions.ResourcesPath"/> -> the project root.
    /// </remarks>
    public class SixnetResourceStringLocalizerFactory : ISixnetStringLocalizerFactory
    {
        #region Fields

        private readonly IResourceNamesCache _resourceNamesCache = new ResourceNamesCache();
        private readonly ConcurrentDictionary<string, SixnetResourceStringLocalizer> _localizerCache = new();
        private readonly string _resourcesRelativePath;
        private readonly string _jsonRelativePath;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISixnetResourceManagerFactory _resourceManagerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="SixnetResourceStringLocalizer"/>.
        /// </summary>
        /// <param name="localizationOptions">The <see cref="IOptions{LocalizationOptions}"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public SixnetResourceStringLocalizerFactory(
            IOptions<SixnetLocalizationOptions> localizationOptions,
            ILoggerFactory loggerFactory,
            ISixnetResourceManagerFactory resourceManagerFactory)
        {
            SixnetDirectThrower.ThrowArgNullIf(localizationOptions == null, nameof(localizationOptions));
            SixnetDirectThrower.ThrowArgNullIf(loggerFactory == null, nameof(loggerFactory));

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
            _jsonRelativePath = localizationOptions.Value.JsonResourcePath ?? string.Empty;
            _loggerFactory = loggerFactory;
            _resourceManagerFactory = resourceManagerFactory;
            if (!string.IsNullOrEmpty(_resourcesRelativePath))
            {
                _resourcesRelativePath = _resourcesRelativePath.Replace(Path.AltDirectorySeparatorChar, '.')
                    .Replace(Path.DirectorySeparatorChar, '.') + ".";
            }
        }

        #endregion

        #region Create localizer

        /// <summary>
        /// Creates a <see cref="SixnetResourceStringLocalizer"/> using the <see cref="Assembly"/> and
        /// <see cref="Type.FullName"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="resourceSource">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="SixnetResourceStringLocalizer"/>.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            return CreateLocalizer(resourceSource);
        }

        /// <summary>
        /// Creates a <see cref="SixnetResourceStringLocalizer"/>.
        /// </summary>
        /// <param name="baseName">The base name of the resource to load strings from.</param>
        /// <param name="location">The location to load resources from.</param>
        /// <returns>The <see cref="SixnetResourceStringLocalizer"/>.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return CreateLocalizer(baseName, location);
        }

        public ISixnetStringLocalizer CreateLocalizer(Type resourceSource)
        {
            SixnetDirectThrower.ThrowArgNullIf(resourceSource == null, nameof(resourceSource));

            var typeInfo = resourceSource.GetTypeInfo();
            var resourcePrefix = GetResourcePrefix(typeInfo);
            var assembly = typeInfo.Assembly;
            return _localizerCache.GetOrAdd(resourcePrefix.ResxBaseName, _ => CreateResourceManagerStringLocalizer(assembly, resourcePrefix));
        }

        public ISixnetStringLocalizer CreateLocalizer(string baseName, string location)
        {
            SixnetDirectThrower.ThrowArgNullIf(baseName == null, nameof(baseName));
            SixnetDirectThrower.ThrowArgNullIf(location == null, nameof(location));

            return _localizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
            {
                var assemblyName = new AssemblyName(location);
                var assembly = Assembly.Load(assemblyName);
                var resourcePrefix = GetResourcePrefix(baseName, location);

                return CreateResourceManagerStringLocalizer(assembly, resourcePrefix);
            });
        }

        /// <summary>Creates a <see cref="SixnetResourceStringLocalizer"/> for the given input.</summary>
        /// <param name="assembly">The assembly to create a <see cref="SixnetResourceStringLocalizer"/> for.</param>
        /// <param name="baseName">The base name of the resource to search for.</param>
        /// <returns>A <see cref="SixnetResourceStringLocalizer"/> for the given <paramref name="assembly"/> and <paramref name="baseName"/>.</returns>
        /// <remarks>This method is virtual for testing purposes only.</remarks>
        protected virtual SixnetResourceStringLocalizer CreateResourceManagerStringLocalizer(
            Assembly assembly,
            ResourcePrefix resourcePrefix)
        {
            return new SixnetResourceStringLocalizer(
                _resourceManagerFactory.Create(resourcePrefix, assembly),
                assembly,
                resourcePrefix,
                _resourceNamesCache,
                _loggerFactory.CreateLogger<SixnetResourceStringLocalizer>());
        }

        #endregion

        #region Get resource prefix

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="typeInfo">The type of the resource to be looked up.</param>
        /// <returns>The prefix for resource lookup.</returns>
        protected virtual ResourcePrefix GetResourcePrefix(TypeInfo typeInfo)
        {
            SixnetDirectThrower.ThrowArgNullIf(typeInfo == null, nameof(typeInfo));
            return GetResourcePrefix(typeInfo, GetRootNamespace(typeInfo.Assembly), GetResourcePath(typeInfo.Assembly));
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="typeInfo">The type of the resource to be looked up.</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <param name="resourcesRelativePath">The folder containing all resources.</param>
        /// <returns>The prefix for resource lookup.</returns>
        /// <remarks>
        /// For the type "Sample.Controllers.Home" if there's a resourceRelativePath return
        /// "Sample.Resourcepath.Controllers.Home" if there isn't one then it would return "Sample.Controllers.Home".
        /// </remarks>
        protected virtual ResourcePrefix GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
        {
            SixnetDirectThrower.ThrowArgNullIf(typeInfo == null, nameof(typeInfo));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrEmpty(baseNamespace), nameof(baseNamespace));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrEmpty(typeInfo.FullName), $"Type '{typeInfo}' must have a non-null type name");

            if (string.IsNullOrEmpty(resourcesRelativePath))
            {
                return new ResourcePrefix()
                {
                    ResxBaseName = typeInfo.FullName,
                    RootNamespace = baseNamespace,
                    ResourcePath = resourcesRelativePath,
                    JsonBaseName = TrimPrefix(typeInfo.FullName, baseNamespace + ".")
                };
            }
            else
            {
                // This expectation is defined by dotnet's automatic resource storage.
                // We have to conform to "{RootNamespace}.{ResourceLocation}.{FullTypeName - RootNamespace}".
                return new ResourcePrefix()
                {
                    RootNamespace = baseNamespace,
                    ResourcePath = resourcesRelativePath,
                    JsonBaseName = TrimPrefix(typeInfo.FullName, baseNamespace + "."),
                    ResxBaseName = baseNamespace + "." + resourcesRelativePath + TrimPrefix(typeInfo.FullName, baseNamespace + ".")
                };
            }
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="baseResourceName">The name of the resource to be looked up</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <returns>The prefix for resource lookup.</returns>
        protected virtual ResourcePrefix GetResourcePrefix(string baseResourceName, string baseNamespace)
        {
            SixnetDirectThrower.ThrowArgNullIf(baseResourceName == null, nameof(baseResourceName));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrEmpty(baseNamespace), nameof(baseNamespace));

            var assemblyName = new AssemblyName(baseNamespace);
            var assembly = Assembly.Load(assemblyName);
            var rootNamespace = GetRootNamespace(assembly);
            var resourceLocation = GetResourcePath(assembly);
            var resourceFilePrefix = TrimPrefix(baseResourceName, baseNamespace + ".");
            return new ResourcePrefix()
            {
                RootNamespace = baseNamespace,
                ResourcePath = resourceLocation,
                ResxBaseName = rootNamespace + "." + resourceLocation + resourceFilePrefix,
                JsonBaseName = resourceFilePrefix,
                JsonResourcePath = _jsonRelativePath
            };
        }

        /// <summary>
        /// Trim prefix
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }

        #endregion

        #region Root namespace

        /// <summary>
        /// Get root namespace
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private string GetRootNamespace(Assembly assembly)
        {
            var rootNamespaceAttribute = GetRootNamespaceAttribute(assembly);
            if (rootNamespaceAttribute != null)
            {
                return rootNamespaceAttribute.RootNamespace;
            }
            return assembly.GetName().Name;
        }

        /// <summary>Gets a <see cref="RootNamespaceAttribute"/> from the provided <see cref="Assembly"/>.</summary>
        /// <param name="assembly">The assembly to get a <see cref="RootNamespaceAttribute"/> from.</param>
        /// <returns>The <see cref="RootNamespaceAttribute"/> associated with the given <see cref="Assembly"/>.</returns>
        /// <remarks>This method is protected and virtual for testing purposes only.</remarks>
        protected virtual RootNamespaceAttribute GetRootNamespaceAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<RootNamespaceAttribute>();
        }

        #endregion

        #region Resource location

        /// <summary>
        /// Get resource path
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private string GetResourcePath(Assembly assembly)
        {
            var resourceLocationAttribute = GetResourceLocationAttribute(assembly);

            // If we don't have an attribute assume all assemblies use the same resource location.
            var resourceLocation = resourceLocationAttribute == null
                ? _resourcesRelativePath
                : resourceLocationAttribute.ResourceLocation + ".";
            resourceLocation = resourceLocation
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            return resourceLocation;
        }

        /// <summary>Gets a <see cref="ResourceLocationAttribute"/> from the provided <see cref="Assembly"/>.</summary>
        /// <param name="assembly">The assembly to get a <see cref="ResourceLocationAttribute"/> from.</param>
        /// <returns>The <see cref="ResourceLocationAttribute"/> associated with the given <see cref="Assembly"/>.</returns>
        /// <remarks>This method is protected and virtual for testing purposes only.</remarks>
        protected virtual ResourceLocationAttribute GetResourceLocationAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<ResourceLocationAttribute>();
        }

        #endregion

    }
}
