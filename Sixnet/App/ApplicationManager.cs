using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sixnet.Logging;

namespace Sixnet.App
{
    /// <summary>
    /// Defines application manager
    /// </summary>
    public static class ApplicationManager
    {
        static ApplicationManager()
        {
            Current = GetCurrentApplicationInfo();
        }

        #region Fields

        /// <summary>
        /// Convention file name patterns
        /// </summary>
        internal static readonly List<string> ConventionFileNamePatterns = new()
        {
            @"\.Entity\.",
            @"\.QueryModel\.",
            @"\.ModuleConfig\.",
            @"\.Domain\.",
            @"\.DataAccess",
            @"\.CacheDataAccess\.",
            @"\.Business",
            @"\.Repository\.",
            @"\.Service\.",
            @"\.AppService",
            @"\.Domain\.",
            @"\.Dto\.",
            @"\.ViewModel\.",
            @"\.Module\.",
            @"\.ApiModel\.",
            @"\.WebApi\.",
            @"AppConfig\."
        };

        /// <summary>
        /// Convention file match regex
        /// </summary>
        internal static Regex ConventionFileMatchRegex = null;

        /// <summary>
        /// Application options
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
        public static string VirtualPath { get; set; } = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="configureApp">Configure application</param>
        internal static void Configure(Action<ApplicationOptions> configureApp = null)
        {
            configureApp?.Invoke(Options);
            ApplicationInitializer.Init();
        }

        /// <summary>
        /// Get the current application root path
        /// </summary>
        /// <returns>Return directory path</returns>
        internal static string GetRootPath()
        {
            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            LogManager.LogInformation(typeof(ApplicationManager).FullName, FrameworkLogEvents.Application.GetApplicationRootPath, $"Application root path:{appPath}");
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
                Type = ApplicationType.Unknown,
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
                    LogManager.LogError(typeof(ApplicationManager).FullName, FrameworkLogEvents.Application.LoadAssemblyFailure, ex, ex.Message);
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
                    FileMatchPattern.None => true,
                    FileMatchPattern.FileNamePrefix => fileMatchOptions.FileNameKeywords?.Any(rn => c.Name.StartsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.FileNameSuffix => fileMatchOptions.FileNameKeywords?.Any(rn => c.Name.EndsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.IncludeFileName => fileMatchOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false,
                    FileMatchPattern.ExcludeFileName => !(fileMatchOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false),
                    FileMatchPattern.IncludeByRegex => new Regex(fileMatchOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.ExcludeByRegex => !new Regex(fileMatchOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.Convention => new Regex($@"^{GetCodeRootPath().Replace(@"\", @"\\")}.*({string.Join("|", ConventionFileNamePatterns.Union(Options.FileMatchOptions.FileRegexPatterns ?? new List<string>(0)))}).*$", RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        }

        #endregion
    }
}
