using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using EZNEW.DependencyInjection;

namespace EZNEW.FileAccess.Configuration
{
    /// <summary>
    /// File access manager
    /// </summary>
    public static class FileAccessManager
    {
        static FileAccessManager()
        {
            var fileAccessConfiguration = ContainerManager.Resolve<IOptions<FileAccessConfiguration>>()?.Value;
            Configure(fileAccessConfiguration);
        }

        #region Properties

        /// <summary>
        /// default file option
        /// </summary>
        static FileAccessOptions Default = null;

        /// <summary>
        /// file options
        /// key:file object name
        /// value:file object access option
        /// </summary>
        static Dictionary<string, FileAccessOptions> FileOptions = new Dictionary<string, FileAccessOptions>();

        #endregion

        #region Methods

        #region Configure file access

        /// <summary>
        /// Configure file access
        /// </summary>
        /// <param name="fileAccessConfiguration">File access configuration</param>
        public static void Configure(FileAccessConfiguration fileAccessConfiguration)
        {
            if (fileAccessConfiguration == null)
            {
                return;
            }
            ConfigureDefault(fileAccessConfiguration.Default);
            ConfigureFileObject(fileAccessConfiguration.FileObjects?.ToArray());
        }

        #endregion

        #region Configure default file access option

        /// <summary>
        /// Configure default file access option
        /// </summary>
        /// <param name="fileAccessOption">File access option</param>
        public static void ConfigureDefault(FileAccessOptions fileAccessOption)
        {
            Default = fileAccessOption;
        }

        #endregion

        #region Configure file object access

        /// <summary>
        /// Configure file object access
        /// </summary>
        /// <param name="fileObjects">File objects</param>
        public static void ConfigureFileObject(params FileObject[] fileObjects)
        {
            if (fileObjects.IsNullOrEmpty())
            {
                return;
            }
            foreach (var fileObject in fileObjects)
            {
                if (fileObject == null || string.IsNullOrWhiteSpace(fileObject.Name) || fileObject.FileAccessOption == null)
                {
                    continue;
                }
                FileOptions[fileObject.Name] = fileObject.FileAccessOption;
            }
        }

        #endregion

        #region Get file full path

        /// <summary>
        /// Get file full path
        /// </summary>
        /// <param name="fileObjectName">File object name</param>
        /// <param name="relativePath">File relative path</param>
        /// <returns>Return the file full path</returns>
        public static string GetFileFullPath(string fileObjectName, string relativePath)
        {
            FileOptions.TryGetValue(fileObjectName, out var fileAccessOption);
            if (fileAccessOption == null)
            {
                fileAccessOption = Default;
            }
            return GetFileFullPath(fileAccessOption, relativePath);
        }

        /// <summary>
        /// Get file full path
        /// </summary>
        /// <param name="fileAccessOption">File access option</param>
        /// <param name="relativePath">File path</param>
        /// <returns>Return file full path</returns>
        public static string GetFileFullPath(FileAccessOptions fileAccessOption, string relativePath)
        {
            return fileAccessOption?.GetFilePath(relativePath) ?? relativePath;
        }

        #endregion

        #endregion
    }
}
