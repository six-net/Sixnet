using System;
using System.Collections.Generic;
using System.IO;
using Sixnet.Algorithm.Selection;

namespace Sixnet.IO.FileAccess
{
    /// <summary>
    /// File access setting
    /// </summary>
    [Serializable]
    public class FileAccessSetting
    {
        #region Fields

        /// <summary>
        /// Root paths
        /// </summary>
        List<string> _rootPaths = null;

        /// <summary>
        /// Data selection provider
        /// </summary>
        SixnetDataSelecter<string> dataSelectionProvider = null;

        #endregion

        #region Properties

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
        public SelectionMatchPattern PathSelectMode
        {
            get; set;
        } = SelectionMatchPattern.EquiprobableRandom;

        #endregion

        #region Methods

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
            dataSelectionProvider = new SixnetDataSelecter<string>(_rootPaths);
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
            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
            {
                return relativePath;
            }
            if (RootPaths.IsNullOrEmpty())
            {
                return relativePath;
            }
            var rootPath = dataSelectionProvider.Get(PathSelectMode);
            string fullPath = Path.Combine(rootPath, relativePath.Trim('\\', '/'));
            return fullPath.Replace("\\", "/");
        }

        #endregion
    }
}
