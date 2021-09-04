using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EZNEW.Development.Query;
using EZNEW.Logging;

namespace EZNEW.Application
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
            @"\.DTO\.",
            @"\.ViewModel\.",
            @"\.Module\.",
            @"\.ApiModel\."
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

        #endregion

        #region Methods

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="configureApplicationDelegate">Configure application delegate</param>
        public static void Configure(Action<ApplicationOptions> configureApplicationDelegate = null)
        {
            configureApplicationDelegate?.Invoke(Options);
            ApplicationInitializer.Init();
        }

        /// <summary>
        /// Get the current application root path
        /// </summary>
        /// <returns>Return directory path</returns>
        internal static string GetRootPath()
        {
            string appPath = Directory.GetCurrentDirectory();
            string binPath = Path.Combine(appPath, "bin");
            if (Directory.Exists(binPath))
            {
                appPath = binPath;
                var debugPath = Path.Combine(appPath, "Debug");
                var releasePath = Path.Combine(appPath, "Release");
                DateTime debugLastWriteTime = DateTime.MinValue;
                DateTime releaseLastWriteTime = DateTime.MinValue;
                bool hasDebug = false;
                bool hasRelease = false;
                if (Directory.Exists(debugPath))
                {
                    hasDebug = true;
                    debugLastWriteTime = Directory.GetLastWriteTime(debugPath);
                }
                if (Directory.Exists(releasePath))
                {
                    hasRelease = false;
                    releaseLastWriteTime = Directory.GetLastWriteTime(releasePath);
                }
                appPath = hasDebug || hasRelease ? debugLastWriteTime >= releaseLastWriteTime ? debugPath : releasePath : appPath;

                var targetFrameworkFolders = Directory.EnumerateDirectories(appPath, "net*", SearchOption.TopDirectoryOnly);
                if (!targetFrameworkFolders.IsNullOrEmpty())
                {
                    var targetFolder = targetFrameworkFolders
                        .Select(c => new DirectoryInfo(c))
                        .OrderByDescending(c => c.LastWriteTime)
                        .FirstOrDefault();
                    appPath = targetFolder.FullName;
                }
            }
            return appPath;
        }

        /// <summary>
        /// Get current application info
        /// </summary>
        /// <returns></returns>
        internal static ApplicationInfo GetCurrentApplicationInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            return new ApplicationInfo()
            {
                Code = "",
                Name = entryAssembly.GetName().Name,
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
            return FilterFiles(new DirectoryInfo(ApplicationManager.RootPath).GetFiles("*.dll", Options.FileMatchOptions.FileSearchOption));
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
            IEnumerable<Type> allTypes = Array.Empty<Type>();
            foreach (var file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file.FullName).GetTypes();
                    allTypes = allTypes.Union(types);
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
                }
            }
            return allTypes;
        }

        /// <summary>
        /// Filter files
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="options">Configuration setting</param>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> FilterFiles(IEnumerable<FileInfo> originalFiles, ApplicationOptions options = null)
        {
            if (originalFiles.IsNullOrEmpty())
            {
                return Array.Empty<FileInfo>();
            }
            options ??= ApplicationManager.Options;
            var fileOptions = options.FileMatchOptions;
            return originalFiles.Where(c =>
            {
                var matched = fileOptions.FileMatchPattern switch
                {
                    FileMatchPattern.None => true,
                    FileMatchPattern.FileNamePrefix => fileOptions.FileNameKeywords?.Any(rn => c.Name.StartsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.FileNameSuffix => fileOptions.FileNameKeywords?.Any(rn => c.Name.EndsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.IncludeFileName => fileOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false,
                    FileMatchPattern.ExcludeFileName => !(fileOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false),
                    FileMatchPattern.IncludeByRegex => new Regex(fileOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.ExcludeByRegex => !new Regex(fileOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.Convention => new Regex($@"^{RootPath.Replace(@"\", @"\\")}.*({string.Join("|", ConventionFileNamePatterns.Union(Options.FileMatchOptions.FileRegexPatterns ?? new List<string>(0)))}).*$", RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        } 

        #endregion
    }
}
