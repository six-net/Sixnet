using System;

namespace Sixnet.IO.FileAccess
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
        /// Gets or sets the file access options
        /// </summary>
        public FileAccessOptions FileAccessOptions { get; set; }
    }
}
