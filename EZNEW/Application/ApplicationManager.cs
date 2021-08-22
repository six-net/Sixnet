using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Gets or sets the information about the currently running application
        /// </summary>
        public static ApplicationInfo Current { get; private set; }

        /// <summary>
        /// Application options
        /// </summary>
        internal static ApplicationOptions Options = new();

        /// <summary>
        /// Gets the current application root path
        /// </summary>
        public static readonly string RootPath = GetRootPath();

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
    }
}
