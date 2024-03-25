using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sixnet.Logging;
using Sixnet.Model;

namespace Sixnet.App
{
    /// <summary>
    /// Defines application manager
    /// </summary>
    public static class SixnetApplication
    {
        #region Constructor

        static SixnetApplication()
        {
            Current = GetCurrentApplicationInfo();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Convention file name patterns
        /// </summary>
        static readonly List<string> _conventionFileNamePatterns = new()
        {
            @"\.Modules\."
        };

        /// <summary>
        /// modules
        /// </summary>
        static List<ISixnetModule> _modules = null;

        /// <summary>
        /// Config models
        /// </summary>
        static List<ISixnetConfigurable> _configModels = null;

        /// <summary>
        /// Default application options
        /// </summary>
        internal static ApplicationOptions Options = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the information about the currently running application
        /// </summary>
        public static ApplicationInfo Current { get; private set; }

        /// <summary>
        /// Gets the current application root path
        /// </summary>
        public static readonly string RootPath = GetRootPath();

        /// <summary>
        /// Gets or sets the virtual path
        /// </summary>
        public static string VirtualPath => Options.VirtualPath;

        #endregion

        #region Methods

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="configure">Configure application</param>
        internal static void Configure(Action<ApplicationOptions> configure = null)
        {
            configure?.Invoke(Options);
        }

        /// <summary>
        /// Init application
        /// </summary>
        internal static void Init()
        {
            ApplicationInitializer.Init();
        }

        /// <summary>
        /// Get the current application root path
        /// </summary>
        /// <returns>Return directory path</returns>
        internal static string GetRootPath()
        {
            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return appPath;
        }

        /// <summary>
        /// Get the code root path
        /// </summary>
        /// <returns></returns>
        internal static string GetCodeRootPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Get current application info
        /// </summary>
        /// <returns></returns>
        internal static ApplicationInfo GetCurrentApplicationInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyName = entryAssembly.GetName().Name;
            return new ApplicationInfo()
            {
                Code = "",
                Name = assemblyName,
                Title = assemblyName,
                Type = SixnetApplicationType.Unknown,
                Version = FileVersionInfo.GetVersionInfo(entryAssembly.Location).FileVersion
            };
        }

        /// <summary>
        /// Get the matched files
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> GetMatchedFiles()
        {
            return FilterFiles(new DirectoryInfo(GetCodeRootPath()).GetFiles("*.dll", Options.FileMatchOptions.FileSearchOption));
        }

        /// <summary>
        /// Get all convention types
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Type> GetAllConventionTypes()
        {
            var files = GetMatchedFiles();
            if (files.IsNullOrEmpty())
            {
                return Array.Empty<Type>();
            }
            var entryAssembly = Assembly.GetEntryAssembly();
            IEnumerable<Type> allTypes = entryAssembly.GetTypes();
            foreach (var file in files)
            {
                try
                {
                    var fileAssembly = Assembly.LoadFrom(file.FullName);
                    if (fileAssembly != null && fileAssembly.FullName != entryAssembly.FullName)
                    {
                        allTypes = allTypes.Union(fileAssembly.GetTypes());
                    }
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(typeof(SixnetApplication).FullName, SixnetLogEvents.Application.LoadAssemblyFailure, ex, ex.Message);
                }
            }
            return allTypes;
        }

        /// <summary>
        /// Filter files
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="options">Application options</param>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> FilterFiles(IEnumerable<FileInfo> originalFiles, ApplicationOptions options = null)
        {
            if (originalFiles.IsNullOrEmpty())
            {
                return Array.Empty<FileInfo>();
            }
            options ??= Options;
            var fileMatchOptions = options.FileMatchOptions;
            return originalFiles.Where(c =>
            {
                var matched = fileMatchOptions.FileMatchPattern switch
                {
                    SixnetFileMatchPattern.None => true,
                    SixnetFileMatchPattern.FileNamePrefix => fileMatchOptions.FileNameKeywords?.Any(rn => c.Name.StartsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    SixnetFileMatchPattern.FileNameSuffix => fileMatchOptions.FileNameKeywords?.Any(rn => c.Name.EndsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    SixnetFileMatchPattern.IncludeFileName => fileMatchOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false,
                    SixnetFileMatchPattern.ExcludeFileName => !(fileMatchOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false),
                    SixnetFileMatchPattern.IncludeByRegex => new Regex(fileMatchOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    SixnetFileMatchPattern.ExcludeByRegex => !new Regex(fileMatchOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    SixnetFileMatchPattern.Convention => new Regex($@"^{GetCodeRootPath().Replace(@"\", @"\\")}.*({string.Join("|", _conventionFileNamePatterns.Union(Options.FileMatchOptions.FileRegexPatterns ?? new List<string>(0)))}).*$", RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        }

        /// <summary>
        /// Add module
        /// </summary>
        /// <param name="module"></param>
        internal static void AddModule(ISixnetModule module)
        {
            if (module != null)
            {
                _modules ??= new List<ISixnetModule>();
                _modules.Add(module);
            }
        }

        /// <summary>
        /// Init modules
        /// </summary>
        internal static void InitModules()
        {
            if (!_modules.IsNullOrEmpty())
            {
                foreach (var configration in _modules)
                {
                    configration?.Init();
                }
                _modules?.Clear();
                _modules = null;
            }
        }

        /// <summary>
        /// Add Configurable model
        /// <param name="module"></param>
        internal static void AddConfigurable(ISixnetConfigurable configurable)
        {
            if (configurable != null)
            {
                _configModels ??= new List<ISixnetConfigurable>();
                _configModels.Add(configurable);
            }
        }

        /// <summary>
        /// Execute configurable
        /// </summary>
        internal static void ExecuteConfigurable()
        {
            if (!_configModels.IsNullOrEmpty())
            {
                foreach (var configurable in _configModels)
                {
                    configurable?.Configure();
                }
                _configModels?.Clear();
                _configModels = null;
            }
        }

        #endregion
    }
}
