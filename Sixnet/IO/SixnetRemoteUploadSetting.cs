// "Company © 2025. All rights reserved."

namespace Sixnet.IO
{
    /// <summary>
    /// Remote remote upload setting
    /// </summary>
    [Serializable]
    public class SixnetRemoteUploadSetting
    {
        /// <summary>
        /// Gets or sets the relative path for file upload
        /// Default value is 'upfile'
        /// </summary>
        public string UploadPath { get; set; } = "upfile";

        /// <summary>
        /// Gets or sets the relative path for file list
        /// Default value is 'filelist'
        /// </summary>
        public string FileListPath { get; set; } = "filelist";

        /// <summary>
        /// Gets or sets the store file path
        /// </summary>
        public string StoreFilePath { get; set; } = "store";

        /// <summary>
        /// Gets or sets server url
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets upload url
        /// </summary>
        /// <returns>Return upload url</returns>
        public string GetUploadUrl()
        {
            if (string.IsNullOrWhiteSpace(UploadPath))
            {
                return Host;
            }
            return string.Format("{0}/{1}", Host.Trim('/'), UploadPath);
        }

        /// <summary>
        /// Gets file list access url
        /// </summary>
        /// <returns>Return file list url</returns>
        public string GetFileListUrl()
        {
            if (string.IsNullOrWhiteSpace(FileListPath))
            {
                return Host;
            }
            return string.Format("{0}/{1}", Host.Trim('/'), FileListPath);
        }

        /// <summary>
        /// Gets file store url
        /// </summary>
        /// <returns></returns>
        public string GetStoreFileUrl()
        {
            if (string.IsNullOrWhiteSpace(StoreFilePath))
            {
                return Host;
            }
            return string.Format("{0}/{1}", Host.Trim('/'), StoreFilePath);
        }
    }
}
