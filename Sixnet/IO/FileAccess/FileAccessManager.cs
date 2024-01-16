using System.Collections.Generic;
using Sixnet.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sixnet.IO.FileAccess
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
        static readonly Dictionary<string, FileAccessOptions> FileOptions = new();

        #endregion

        #region Methods

        #region Configure file access

        /// <summary>
        /// Configure file access
        /// </summary>
        /// <param name="fileAccessConfiguration">File access configuration</param>
        public static void Configure(FileAccessConfiguration fileAccessConfiguration)
        {
            if (fileAccessConfiguration != null)
            {
                ConfigureDefault(fileAccessConfiguration.Default);
                ConfigureFileObject(fileAccessConfiguration.FileObjects?.ToArray());
            }
        }

        #endregion

        #region Configure default file access option

        /// <summary>
        /// Configure default file access behavior
        /// </summary>
        /// <param name="fileAccessOptions">File access option</param>
        public static void ConfigureDefault(FileAccessOptions fileAccessOptions)
        {
            Default = fileAccessOptions;
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
                if (fileObject == null || string.IsNullOrWhiteSpace(fileObject.Name) || fileObject.FileAccessOptions == null)
                {
                    continue;
                }
                FileOptions[fileObject.Name] = fileObject.FileAccessOptions;
            }
        }

        /// <summary>
        /// Gets file access options
        /// </summary>
        /// <param name="fileObjectName">File object name</param>
        /// <returns></returns>
        public static FileAccessOptions GetFileAccessOptions(string fileObjectName)
        {
            FileOptions.TryGetValue(fileObjectName, out var fileAccessOptions);
            if (fileAccessOptions == null)
            {
                fileAccessOptions = Default;
            }
            return fileAccessOptions;
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
            FileOptions.TryGetValue(fileObjectName, out var fileAccessOptions);
            fileAccessOptions ??= Default;
            return GetFileFullPath(fileAccessOptions, relativePath);
        }

        /// <summary>
        /// Get file full path
        /// </summary>
        /// <param name="fileAccessOptions">File access options</param>
        /// <param name="relativePath">File path</param>
        /// <returns>Return file full path</returns>
        public static string GetFileFullPath(FileAccessOptions fileAccessOptions, string relativePath)
        {
            return fileAccessOptions?.GetFilePath(relativePath) ?? relativePath;
        }

        #endregion

        #endregion
    }
}
