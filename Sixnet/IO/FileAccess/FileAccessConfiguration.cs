using System;
using System.Collections.Generic;

namespace Sixnet.IO.FileAccess
{
    /// <summary>
    /// File access configuration
    /// </summary>
    [Serializable]
    public class FileAccessConfiguration
    {
        /// <summary>
        /// Gets or sets the default file access option
        /// </summary>
        public FileAccessOptions Default { get; set; }

        /// <summary>
        /// Gets or sets the file objects configuration
        /// </summary>
        public List<FileObject> FileObjects { get; set; }
    }
}
