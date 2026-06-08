// "Company © 2025. All rights reserved."

using System.IO;

using Sixnet.Algorithm.Selection;

namespace Sixnet.IO
{
    /// <summary>
    /// File setting
    /// </summary>
    [Serializable]
    public class SixnetFileSetting
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

        /// <summary>
        /// remote selection provider
        /// </summary>
        SixnetDataSelecter<SixnetRemoteUploadSetting> remoteUploadSettingSelecter = null;

        /// <summary>
        /// remote upload setting
        /// </summary>
        List<SixnetRemoteUploadSetting> remoteUploadSettings = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets file acctionn root paths
        /// </summary>
        public List<string> AccessRootPaths
        {
            get
            {
                return _rootPaths;
            }
            set
            {
                SetAccessRootPaths(value);
            }
        }

        /// <summary>
        /// Gets or sets the access path select mode
        /// </summary>
        public SixnetSelectionMatchPattern AccessPathSelectionMode
        {
            get; set;
        } = SixnetSelectionMatchPattern.EquiprobableRandom;

        /// <summary>
        /// Gets or sets whether to use remote upload
        /// Default is false
        /// </summary>
        public bool UseRemoteUpload { get; set; }

        /// <summary>
        /// Whether disable default folder group
        /// Default is false
        /// </summary>
        public bool DisableDefaultFolderGroup { get; set; }

        /// <summary>
        /// Gets or sets whether classification file by date folder.
        /// default is true
        /// </summary>
        public bool UseDateGroupFolder { get; set; } = true;

        /// <summary>
        /// Gets or sets file uplaod path
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        /// Gets or sets the store path
        /// </summary>
        public string StorePath { get; set; }

        /// <summary>
        /// Whether save to the temp folder first
        /// Default is true
        /// </summary>
        public bool UploadToTempFirst { get; set; } = true;

        /// <summary>
        /// Gets or sets whether rename file
        /// Default is true
        /// </summary>
        public bool Rename { get; set; } = true;

        /// <summary>
        /// Gets or sets remote upload setting
        /// </summary>
        public List<SixnetRemoteUploadSetting> RemoteUploadSettings
        {
            get
            {
                return remoteUploadSettings;
            }
            set
            {
                remoteUploadSettings = value;
                remoteUploadSettingSelecter = new SixnetDataSelecter<SixnetRemoteUploadSetting>(remoteUploadSettings);
            }
        }

        /// <summary>
        /// Gets or sets the remove upload setting choice pattern
        /// Default value is 'Equiprobable random' pattern
        /// </summary>
        public SixnetSelectionMatchPattern RemoteUploadSettingSelectionPattern { get; set; } = SixnetSelectionMatchPattern.EquiprobableRandom;

        #endregion

        #region Methods

        /// <summary>
        /// Set root paths
        /// </summary>
        /// <param name="accessRootPaths">Access root paths</param>
        void SetAccessRootPaths(List<string> accessRootPaths)
        {
            if (accessRootPaths.IsNullOrEmpty())
            {
                return;
            }
            _rootPaths = accessRootPaths;
            dataSelectionProvider = new SixnetDataSelecter<string>(_rootPaths);
        }

        /// <summary>
        /// Get file access path
        /// </summary>
        /// <param name="relativePath">Relative file path</param>
        /// <returns>Return the file full path</returns>
        public string GetFileAccessPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }
            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
            {
                return relativePath;
            }
            if (AccessRootPaths.IsNullOrEmpty())
            {
                return relativePath;
            }
            var rootPath = dataSelectionProvider.Get(AccessPathSelectionMode);
            string fullPath = Path.Combine(rootPath, relativePath.Trim('\\', '/'));
            return fullPath.Replace("\\", "/");
        }

        /// <summary>
        /// Gets remote setting
        /// </summary>
        /// <returns></returns>
        public SixnetRemoteUploadSetting GetRemoteUploadSetting()
        {
            if (RemoteUploadSettings.IsNullOrEmpty())
            {
                return null;
            }
            int serverCount = RemoteUploadSettings.Count;
            if (serverCount == 1)
            {
                return RemoteUploadSettings[0];
            }
            return remoteUploadSettingSelecter.Get(RemoteUploadSettingSelectionPattern);
        }

        #endregion
    }
}
