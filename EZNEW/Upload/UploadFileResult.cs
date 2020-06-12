using System;

namespace EZNEW.Upload
{
    /// <summary>
    /// Upload file result
    /// </summary>
    [Serializable]
    public class UploadFileResult
    {
        /// <summary>
        /// relative path
        /// </summary>
        string relativePath = string.Empty;

        /// <summary>
        /// full path
        /// </summary>
        string fullPath = string.Empty;

        #region Properties

        /// <summary>
        /// Gets or sets the file suffix
        /// </summary>
        public string Suffix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original file name
        /// </summary>
        public string OriginalFileName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative path
        /// </summary>
        public string RelativePath
        {
            get
            {
                return relativePath;
            }
            set
            {
                relativePath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// Gets or sets the full path
        /// </summary>
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// Gets or sets the upload date
        /// </summary>
        public DateTimeOffset UploadDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the upload location
        /// </summary>
        public UploadTarget Target
        {
            get; set;
        }

        #endregion
    }
}
