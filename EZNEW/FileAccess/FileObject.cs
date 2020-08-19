using System;

namespace EZNEW.FileAccess.Configuration
{
    /// <summary>
    /// File object
    /// </summary>
    [Serializable]
    public class FileObject
    {
        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file access option
        /// </summary>
        public FileAccessOption FileAccessOption { get; set; }
    }
}
