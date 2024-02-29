using System.Collections.Generic;
using Sixnet.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Sixnet.IO.FileAccess
{
    /// <summary>
    /// File accessor
    /// </summary>
    public static class SixnetFileAccessor
    {
        #region Gets file access setting

        /// <summary>
        /// Gets file access setting
        /// </summary>
        /// <param name="fileObjectName">File object name</param>
        /// <returns></returns>
        public static FileAccessSetting GetFileAccessSetting(string fileObjectName)
        {
            FileAccessSetting fileAccessSetting = null;
            var fileAccessOptions = SixnetContainer.GetOptions<FileAccessOptions>();
            fileAccessOptions?.Files?.TryGetValue(fileObjectName, out fileAccessSetting);
            fileAccessSetting ??= fileAccessOptions?.Default ?? new FileAccessSetting();
            return fileAccessSetting;
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
            var fileAccessSetting = GetFileAccessSetting(fileObjectName);
            return GetFileFullPath(fileAccessSetting, relativePath);
        }

        /// <summary>
        /// Get file full path
        /// </summary>
        /// <param name="fileAccessSetting">File access setting</param>
        /// <param name="relativePath">File path</param>
        /// <returns>Return file full path</returns>
        public static string GetFileFullPath(FileAccessSetting fileAccessSetting, string relativePath)
        {
            return fileAccessSetting?.GetFilePath(relativePath) ?? relativePath;
        }

        #endregion
    }
}
