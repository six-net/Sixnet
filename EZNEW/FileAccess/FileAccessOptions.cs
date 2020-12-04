using System;
using System.Collections.Generic;
using System.IO;
using EZNEW.Selection;

namespace EZNEW.FileAccess.Configuration
{
    /// <summary>
    /// File access options
    /// </summary>
    [Serializable]
    public class FileAccessOptions
    {
        /// <summary>
        /// Root paths
        /// </summary>
        List<string> _rootPaths = null;

        /// <summary>
        /// Data selection provider
        /// </summary>
        DataSelectionProvider<string> dataSelectionProvider = null;

        /// <summary>
        /// Gets or sets file root paths
        /// </summary>
        public List<string> RootPaths
        {
            get
            {
                return _rootPaths;
            }
            set
            {
                SetRootPaths(value);
            }
        }

        /// <summary>
        /// Gets or sets the path select mode
        /// </summary>
        public SelectMatchMode PathSelectMode
        {
            get; set;
        } = SelectMatchMode.EquiprobableRandom;

        /// <summary>
        /// Set root paths
        /// </summary>
        /// <param name="rootPaths">root paths</param>
        void SetRootPaths(List<string> rootPaths)
        {
            if (rootPaths.IsNullOrEmpty())
            {
                return;
            }
            _rootPaths = rootPaths;
            dataSelectionProvider = new DataSelectionProvider<string>(_rootPaths);
        }

        /// <summary>
        /// Get file path
        /// </summary>
        /// <param name="relativePath">Relative file path</param>
        /// <returns>Return the file full path</returns>
        public string GetFilePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }
            if (RootPaths.IsNullOrEmpty())
            {
                return relativePath;
            }
            var rootPath = dataSelectionProvider.Get(PathSelectMode);
            string fullPath = Path.Combine(rootPath, relativePath);
            return fullPath.Replace("\\", "/");
        }
    }
}
