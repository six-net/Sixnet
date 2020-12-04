using System;

namespace EZNEW.Upload
{
    /// <summary>
    /// Remote upload server options
    /// </summary>
    [Serializable]
    public class RemoteServerOptions
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
    }
}
