using System;

namespace EZNEW.Upload
{
    /// <summary>
    /// Upload file info
    /// </summary>
    [Serializable]
    public class UploadFile
    {
        /// <summary>
        /// Gets or sets the upload object name
        /// </summary>
        public string UploadObjectName { get; set; }

        /// <summary>
        /// Gets or sets file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets save folder
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets file suffix
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets whether to rename the file
        /// </summary>
        public bool Rename { get; set; }
    }
}
