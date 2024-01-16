using System;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload file info
    /// </summary>
    [Serializable]
    public class UploadFile
    {
        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string ObjectName { get; set; }

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
        /// Whether to rename the file
        /// </summary>
        public bool Rename { get; set; }

        /// <summary>
        /// Gets or sets the file content
        /// </summary>
        public byte[] FileContent { get; set; }
    }
}
