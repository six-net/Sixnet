using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;

namespace EZNEW.Application
{
    /// <summary>
    /// Provides access to and management of application information
    /// </summary>
    public static class ApplicationManager
    {
        /// <summary>
        /// Gets or sets the information about the currently running application
        /// </summary>
        public static ApplicationInfo Current { get; set; }

        /// <summary>
        /// Gets the application executable direc
        /// </summary>
        public static readonly string ApplicationExecutableDirectory = GetApplicationExecutableDirectory();

        /// <summary>
        /// Get the current application executable directory
        /// </summary>
        /// <returns>Return directory path</returns>
        internal static string GetApplicationExecutableDirectory()
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
            }
            return appPath;
        }
    }
}
